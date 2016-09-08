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
    }
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
