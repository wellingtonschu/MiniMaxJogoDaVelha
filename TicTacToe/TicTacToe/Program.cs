using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TicTacToe
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
                        c = 'X';
                    else if (v == EntradaGrade.JogadorO)
                        c = 'O';
                    sb.Append(c);
                }
                sb.Append('\n');
            }
            sb.AppendFormat("placar={0},ret={1},{2}", m_Placar, PlacarRecursivo, m_TurnoJogadorX);
            return sb.ToString();
        }

        public Tabuleiro ObtemFilhoNaPosicao(int x, int y)
        {
            int i = x + y * 3;
            EntradaGrade[] novosValores = (EntradaGrade[])m_Valores.Clone();

            if (m_Valores[i] != EntradaGrade.Vazio)
                throw new Exception(string.Format("Indice inválido [{0},{1}] já possui valor de {2}", x, y, m_Valores[i]));

            novosValores[i] = m_TurnoJogadorX ? EntradaGrade.JogadorX : EntradaGrade.JogadorO;
            return new Tabuleiro(novosValores, !m_TurnoJogadorX);
        }

        public bool noFinal()
        {
            if (GameOver)
                return true;
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

        //http://en.wikipedia.org/wiki/Alpha-beta_pruning
        public int MiniMaxCurto(int profundidade, int alpha, int beta, out Tabuleiro filhoComMax)
        {
            filhoComMax = null;
            if (profundidade == 0 || noFinal())
            {
                //When it is turn for PlayO, we need to find the minimum score.
                PlacarRecursivo = m_Placar;
                return m_TurnoJogadorX ? m_Placar : -m_Placar;
            }

            foreach (Tabuleiro cur in ObtemFilho())
            {
                Tabuleiro burro;
                int placar = -cur.MiniMaxCurto(profundidade - 1, -beta, -alpha, out burro);
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

        //http://www.ocf.berkeley.edu/~yosenl/extras/alphabeta/alphabeta.html
        public int MiniMax(int profundidade, bool precisaMax, int alpha, int beta, out Tabuleiro filhoComMax)
        {
            filhoComMax = null;
            System.Diagnostics.Debug.Assert(m_TurnoJogadorX == precisaMax);
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

        public Tabuleiro EncontraProximoMovimento(int profundidade)
        {
            Tabuleiro ret01 = null;
            Tabuleiro ret02 = null;
            MiniMaxCurto(profundidade, int.MinValue + 1, int.MaxValue - 1, out ret02);
            MiniMax(profundidade, m_TurnoJogadorX, int.MinValue + 1, int.MaxValue - 1, out ret01);

            //compare the two versions of MiniMax give the same results
            if (!MesmoTabuleiro(ret01, ret02, true))
            {
                Console.WriteLine("ret={0}\n,!= ret1={1},\ncur={2}", ret01, ret02, this);
                throw new Exception("Two MinMax functions don't match.");
            }
            return ret01;
        }

        int ObtemPlacarParaUmaLinha(EntradaGrade[] valores)
        {
            int contadorX = 0;
            int contadorO = 0;
            foreach (EntradaGrade v in valores)
            {
                if (v == EntradaGrade.JogadorX)
                {
                    contadorX++;
                } else if (v == EntradaGrade.JogadorO)
                {
                    contadorO++;
                }
            }

            if (contadorO == 3 || contadorX == 3)
            {
                GameOver = true;
            }

            //The player who has turn should have more advantage.
            //What we should have done
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
                p = t.ExecAcao(p);
                int j = p.x + p.y * 3;
                System.Diagnostics.Debug.Assert(valores[j] == EntradaGrade.Vazio);
                valores[j] = this.m_Valores[i];
            }
            return new Tabuleiro(valores, m_TurnoJogadorX);
        }

        static bool MesmoTabuleiro(Tabuleiro a, Tabuleiro b, bool comparaPlacarRecursivo)
        {
            if (a == b)
            {
                return true;
            }
            if (a == null || b == null)
            {
                return false;
            }
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
            if (comparaPlacarRecursivo && Math.Abs(a.PlacarRecursivo) != Math.Abs(b.PlacarRecursivo))
            {
                return false;
            }
            return true;
        }

        public static bool TabuleiroSimilar(Tabuleiro a, Tabuleiro b)
        {
            if (MesmoTabuleiro(a, b, false))
            {
                return true;
            }
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

    class JogoDaVelha
    {
        public Tabuleiro Atual
        {
            get;
            private set;
        }
        Tabuleiro inicia;

        public JogoDaVelha()
        {
            EntradaGrade[] valores = Enumerable.Repeat(EntradaGrade.Vazio, 9).ToArray();
            inicia = new Tabuleiro(valores, true);
            Atual = inicia;
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
