using System;
using System.Collections.Generic;

namespace JogoDaVelhaIA
{
    class Program
    {
        static void Main(string[] args)
        {
            JogoDaVelha jogo = new JogoDaVelha();
            Queue<ClasseMiniMax> q = new Queue<ClasseMiniMax>();
            int total = 0;
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
                {
                    Console.WriteLine("JogadorO ganhou.");
                }
                else if (jogo.Atual.PlacarRecursivo > 200)
                {
                    Console.WriteLine("JogadorX ganhou.");
                }
                else
                {
                    Console.WriteLine("Empate.");
                }
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
