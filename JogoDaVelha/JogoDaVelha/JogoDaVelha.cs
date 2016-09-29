using System;
using System.Linq;

namespace JogoDaVelhaIA
{
    class JogoDaVelha
    {
        public ClasseMiniMax Atual
        {
            get;
            private set;
        }
        ClasseMiniMax inicia;
        public JogoDaVelha()
        {
            EntradaGrade[] valores = Enumerable.Repeat(EntradaGrade.Vazio, 9).ToArray();
            inicia = new ClasseMiniMax(valores, true);
            Atual = inicia;
        }
        public void MovimentoDoComputador(int profundidade)
        {
            ClasseMiniMax proximo = Atual.EncontraProximoMovimento(profundidade);
            if (proximo != null)
            {
                Atual = proximo;
            }
        }
        public ClasseMiniMax ObtemNoInicial()
        {
            return inicia;
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
                    Console.WriteLine("O tabuleiro atual é:\n{0}\n Informe valor em x:[0-1-2]", Atual);
                    int x = int.Parse(Console.ReadLine());
                    Console.WriteLine("Informe valor em y:[0-1-2]");
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
