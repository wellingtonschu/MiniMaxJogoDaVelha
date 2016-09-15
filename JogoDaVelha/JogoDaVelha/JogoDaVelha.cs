using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JogoDaVelhaIA
{
    class JogoDaVelha
    {
        public Tabuleiro Atual
        {
            get;
            private set;
        }
        Tabuleiro inicial;
        public JogoDaVelha()
        {
            EntradaGrade[] valores = Enumerable.Repeat(EntradaGrade.Vazio, 9).ToArray();
            inicial = new Tabuleiro(valores, true);
            Atual = inicial;
        }
        public void MovimentoDoComputador(int profundidade)
        {
            Tabuleiro proximo = Atual.EncontraProximoMovimento(profundidade);
            if (proximo != null)
            {
                Atual = proximo;
            }
        }
        public Tabuleiro ObtemNoInicial()
        {
            return inicial;
        }
        public void ObtemProximoMovimentoDoUsuario()
        {
            if (Atual.noFinal())
            {
                return;
            }
            while (true)
            {
                try
                {
                    Console.WriteLine("O nó atual é:\n{0}\n Informe valor em x:[0-2]", Atual);
                    int x = int.Parse(Console.ReadLine());
                    Console.WriteLine("Informe valor em y:[0-2]");
                    int y = int.Parse(Console.ReadLine());
                    Console.WriteLine("x={0},y={1}", x, y);
                    Atual = Atual.ObtemFilhoNaPosicao(x, y);
                    Console.WriteLine(Atual);
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
