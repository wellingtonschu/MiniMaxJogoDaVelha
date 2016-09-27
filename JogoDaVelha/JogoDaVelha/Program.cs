using System;
using System.Collections.Generic;

namespace JogoDaVelhaIA
{
    class Program
    {
        static void Main(string[] args)
        {
            JogoDaVelha jogo = new JogoDaVelha();
            Console.WriteLine("Posição para vitoria do JogadorO:");
            List<Tabuleiro> historico = new List<Tabuleiro>();
            Queue<Tabuleiro> q = new Queue<Tabuleiro>();
            q.Enqueue(jogo.ObtemNoInicial());
            int total = 0;
            while (q.Count > 0)
            {
                Tabuleiro b = q.Dequeue();
                Tabuleiro proximo = b.EncontraProximoMovimento(9);
                if (Math.Abs(b.PlacarRecursivo) >= 200 && proximo != null)
                {
                    if (b.PlacarRecursivo < 0 && !proximo.GameOver && historico.Find(x => Tabuleiro.TabuleiroSimilar(x, b)) == null)
                    {
                        historico.Add(b);
                        Console.WriteLine("[{0}] Vencedor {1}:\n{2}, proximo movimento:\n{3}", total, b.PlacarRecursivo < 0 ? "JogadorO" : "JogadorX", b, proximo);
                        total++;
                    }
                }
                else
                {
                    foreach (Tabuleiro c in b.ObtemFilho())
                    {
                        q.Enqueue(c);
                    }
                }
            }

            bool parada = false;
            while (!parada)
            {
                bool primeiroUsuario = false;
                jogo = new JogoDaVelha();
                Console.WriteLine("Usuário contra computador, quer jogar primeiro? [s/n]");
                if (Console.ReadLine().StartsWith("s", StringComparison.InvariantCultureIgnoreCase))
                {
                    primeiroUsuario = true;
                }

                int profundidade = 8;
                Console.WriteLine("Selecione o nivel:[1..8]. 1 é facil, 8 é dificil");
                int.TryParse(Console.ReadLine(), out profundidade);

                Console.WriteLine("{0} Joga primeiro, nivel={1}", primeiroUsuario ? "Usuario" : "Computador", profundidade);

                while (!jogo.Atual.noFinal())
                {
                    if (primeiroUsuario)
                    {
                        jogo.ObtemProximoMovimentoDoUsuario();
                        jogo.MovimentoDoComputador(profundidade);
                    }
                    else
                    {
                        jogo.MovimentoDoComputador(profundidade);
                        jogo.ObtemProximoMovimentoDoUsuario();
                    }
                }
                Console.WriteLine("O resultado final é \n" + jogo.Atual);
                if (jogo.Atual.PlacarRecursivo < -200)
                    Console.WriteLine("JogadorO ganhou.");
                else if (jogo.Atual.PlacarRecursivo > 200)
                    Console.WriteLine("JogadorX ganhou.");
                else
                    Console.WriteLine("Empate.");

                Console.WriteLine("Jogar novamente[s/n]");
                if (!Console.ReadLine().StartsWith("s", StringComparison.InvariantCultureIgnoreCase))
                {
                    parada = true;
                }
            }

            Console.WriteLine("Fim");
        }
    }
}
