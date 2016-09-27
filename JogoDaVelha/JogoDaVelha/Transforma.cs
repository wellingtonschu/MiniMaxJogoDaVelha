using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JogoDaVelhaIA
{
    struct Ponto
    {
        public int x;
        public int y;
        public Ponto(int x0, int y0)
        {
            x = x0;
            y = y0;
        }
    }

    class Transforma
    {
        const int Tamanho = 3;
        delegate Ponto TransformaFunc(Ponto p);
        public static Ponto Rotaciona90Graus(Ponto p)
        {
            return new Ponto(Tamanho - p.y - 1, p.x);
        }
        public static Ponto EspelhoX(Ponto p)
        {
            return new Ponto(Tamanho - p.x - 1, p.y);
        }
        public static Ponto EspelhoY(Ponto p)
        {
            return new Ponto(p.x, Tamanho - p.y - 1);
        }

        List<TransformaFunc> acoes = new List<TransformaFunc>();
        public Ponto ExecAcao(Ponto p)
        {
            foreach (TransformaFunc f in acoes)
            {
                if (f != null)
                {
                    p = f(p);
                }
            }

            return p;
        }

        Transforma(TransformaFunc op, TransformaFunc[] ops)
        {
            if (op != null)
            {
                acoes.Add(op);
            }
            if (ops != null && ops.Length > 0)
            {
                acoes.AddRange(ops);
            }
        }
        public static List<Transforma> s_Transforma = new List<Transforma>();
        static Transforma()
        {
            for (int i = 0; i < 4; i++)
            {
                TransformaFunc[] ops = Enumerable.Repeat<TransformaFunc>(Rotaciona90Graus, i).ToArray();
                s_Transforma.Add(new Transforma(null, ops));
                s_Transforma.Add(new Transforma(EspelhoX, ops));
                s_Transforma.Add(new Transforma(EspelhoY, ops));
            }
        }
    }
}
