using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JogoDaVelha
{
    enum EntradaGrade : byte
    {
        Vazio,
        JogadorX,
        JogadorO
    }

    sealed class Tabuleiro
    {
        EntradaGrade[] m_Valores;
        int m_Placar;
        bool m_TurnoJogadorX;
        public int PlacarRecursivo
        {
            get;
            private set;
        }
        public bool GameOver
        {
            get;
            private set;
        }
        public Tabuleiro(EntradaGrade[] valores, bool turnoJogadorX)
        {
            m_TurnoJogadorX = turnoJogadorX;
            m_Valores = valores;
            PlacarComputado();
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    EntradaGrade v = m_Valores[i * 3 + j];
                    char c = '-';

                    if (v == EntradaGrade.JogadorX)
                    {
                        c = 'X';
                    }
                    else if (v == EntradaGrade.JogadorO)
                    {
                        c = 'O';
                    }
                    sb.Append(c);
                }
                sb.Append('\n');
            }
            sb.AppendFormat("Placar={0}, Ret={1},{2}", m_Placar, PlacarRecursivo, m_TurnoJogadorX);
            return sb.ToString();
        }
        public Tabuleiro ObtemFilhoNaPosicao(int x, int y)
        {
            int i = x + y * 3;
            EntradaGrade[] novosValores = (EntradaGrade[])m_Valores.Clone();

            if (m_Valores[i] != EntradaGrade.Vazio) throw new Exception(string.Format("Indice inválido [{0},{1}] já possui valor de {2}", x, y, m_Valores[i]));

            novosValores[i] = m_TurnoJogadorX ? EntradaGrade.JogadorX : EntradaGrade.JogadorO;

            return new Tabuleiro(novosValores, !m_TurnoJogadorX);
        }
        public bool noFinal()
        {
            if (GameOver)
            {
                return true;
            }
            foreach (EntradaGrade v in m_Valores)
            {
                if (v == EntradaGrade.Vazio)
                    return false;
            }
            return true;
        }
        public IEnumerable<Tabuleiro> ObtemFilho()
        {
            for (int i = 0; i < m_Valores.Length; i++)
            {
                if (m_Valores[i] == EntradaGrade.Vazio)
                {
                    EntradaGrade[] novosValores = (EntradaGrade[])m_Valores.Clone();
                    novosValores[i] = m_TurnoJogadorX ? EntradaGrade.JogadorX : EntradaGrade.JogadorO;
                    yield return new Tabuleiro(novosValores, !m_TurnoJogadorX);
                }
            }
        }
        public int MiniMaxCurto(int profundidade, int alpha, int beta, out Tabuleiro filhoComMax)
        {
            filhoComMax = null;
            if (profundidade == 0 || noFinal())
            {
                PlacarRecursivo = m_Placar;
                return m_TurnoJogadorX ? m_Placar : -m_Placar;
            }
            foreach(Tabuleiro cur in ObtemFilho())
            {
                Tabuleiro burro;
                int placar = -MiniMaxCurto(profundidade - 1, -beta, -alpha, out burro);
                if (alpha < placar)
                {
                    alpha = placar;
                    filhoComMax = cur;
                    if (alpha >= beta)
                    {
                        break;
                    }
                }
            }
            PlacarRecursivo = alpha;
            return alpha;
        }
        public int MiniMax(int profundidade, bool precisaMax, int alpha, int beta, out Tabuleiro filhoComMax)
        {
            filhoComMax = null;
            System.Diagnostics.Debug.Assert(m_TurnoJogadorX = precisaMax);
            if (profundidade == 0 || noFinal())
            {
                PlacarRecursivo = m_Placar;
                return m_Placar;
            }
            foreach (Tabuleiro cur in ObtemFilho())
            {
                Tabuleiro burro;
                int placar = cur.MiniMax(profundidade - 1, !precisaMax, alpha, beta, out burro);
                if (!precisaMax)
                {
                    if (beta > placar)
                    {
                        beta = placar;
                        filhoComMax = cur;
                        if (alpha >= beta)
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (alpha < placar)
                    {
                        alpha = placar;
                        filhoComMax = cur;
                        if (alpha >= beta)
                        {
                            break;
                        }
                    }
                }
            }
            PlacarRecursivo = precisaMax ? alpha : beta;
            return PlacarRecursivo;
        }
        public Tabuleiro EncontraProximoMovimento (int profundidade)
        {
            Tabuleiro ret01 = null;
            Tabuleiro ret02 = null;
            MiniMaxCurto (profundidade, int.MinValue + 1, int.MaxValue - 1, out ret02);
            MiniMax(profundidade, m_TurnoJogadorX, int.MinValue + 1, int.MaxValue - 1, out ret01);

            //Compara se as duas versãos do algoritmo MinMAx possuem o mesmo resultado.
            if (!MesmoTabuleiro(ret01, ret02, true))
            {
                Console.WriteLine("ret01={0}\n,!= ret02={1},\n cur={2}", ret01, ret02, this);
                throw new Exception("Funções MinMax e MinMaxCurto não batem");
            }
            return ret01;
        }
        int ObtemPlacarParaUmaLinha (EntradaGrade[] valores)
        {
            int contadorX = 0;
            int contadorO = 0;
            foreach (EntradaGrade v in valores)
            {
                if (v == EntradaGrade.JogadorX)
                {
                    contadorX++;
                }
                else if (v == EntradaGrade.JogadorO)
                {
                    contadorO++;
                }
            }

            if (contadorO == 3 || contadorX == 3)
            {
                GameOver = true;
            }

            int vantagem = 1;
            if (contadorO == 0)
            {
                if (m_TurnoJogadorX)
                {
                    vantagem = 3;
                }
                return (int)System.Math.Pow(10, contadorX) * vantagem;
            }
            else if (contadorX == 0)
            {
                if (!m_TurnoJogadorX)
                {
                    vantagem = 3;
                }
                return -(int)System.Math.Pow(10, contadorO) * vantagem;
            }
            return 0;
        }
        void PlacarComputado()
        {
            int ret = 0;
            int[,] linhas =
            {
                { 0, 1, 2 },
                { 3, 4, 5 },
                { 6, 7, 8 },
                { 0, 3, 6 },
                { 1, 4, 7 },
                { 2, 5, 8 },
                { 0, 4, 8 },
                { 2, 4, 6 }
            };
            for (int i = linhas.GetLowerBound(0); i <= linhas.GetUpperBound(0); i++)
            {
                ret += ObtemPlacarParaUmaLinha(new EntradaGrade[] { m_Valores[linhas[i, 0]], m_Valores[linhas[i, 1]], m_Valores[linhas[i, 2]] });
            }
            m_Placar = ret;
        }
        public Tabuleiro TransformaTabuleiro(Transforma t)
        {
            EntradaGrade[] valores = Enumerable.Repeat(EntradaGrade.Vazio, 9).ToArray();
            for (int i = 0; i < 9; i++)
            {
                Ponto p = new Ponto(i % 3, i / 3);
                p = t.ActOn(p);
                int j = p.x + p.y * 3;
                System.Diagnostics.Debug.Assert(valores[j] == EntradaGrade.Vazio);
                valores[j] = this.m_Valores[i];
            }
            return new Tabuleiro(valores, m_TurnoJogadorX);
        }
        static bool MesmoTabuleiro(Tabuleiro a, Tabuleiro b, bool comparePlacarRecursivo)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            for (int i = 0; i < a.m_Valores.Length; i++)
            {
                if (a.m_Valores[i] != b.m_Valores[i])
                {
                    return false;
                }
            }

            if (a.m_Placar != b.m_Placar)
            {
                return false;
            }

            if (comparePlacarRecursivo && Math.Abs(a.PlacarRecursivo) != Math.Abs(b.PlacarRecursivo))
            {
                return false;
            }
            return true;
        }
        public static bool TabuleiroSimilar(Tabuleiro a, Tabuleiro b)
        {
            if (MesmoTabuleiro(a, b, false))
                return true;

            foreach (Transforma t in Transforma.s_Transforma)
            {
                Tabuleiro novoB = b.TransformaTabuleiro(t);
                if (MesmoTabuleiro(a, novoB, false))
                {
                    return true;
                }
            }
            return false;
        }
    }
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

    }
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
