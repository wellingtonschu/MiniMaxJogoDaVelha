// Saiba mais sobre F# em http://fsharp.org
// Veja o projeto 'F# Tutorial' para obter mais ajuda.

let linhas = [[1;2;3];
            [4;5;6];
            [7;8;9];
            [1;4;7];
            [2;5;8];
            [3;6;9];
            [1;5;9];
            [3;5;7];]

let Contains numero = List.exists (fun n -> n = numero)

let ContainsList lista = List.forall (fun n -> lista |> Contains n)

let ExceptList lista = List.filter (fun n -> not (lista |> Contains n))

let Available (jogador: int list) (adversario: int list) =
    [1..9]
    |> ExceptList (List.append jogador adversario) 

let vitoria (quadrantes: int list) = 
    linhas |> List.exists (fun w -> ContainsList quadrantes w)

let empate jogador adversario =
    List.isEmpty (Available jogador adversario)

let rec placar (jogador: int list) (adversario: int list) =
    if (vitoria jogador) then 1
    else if (empate jogador adversario) then 0
    else
        let melhorMovimentoDoAdversario = melhorMovimento adversario jogador
        let novaPosicaoDoAdversario = melhorMovimentoDoAdversario::adversario
        -placar novaPosicaoDoAdversario jogador

and melhorMovimento (jogador: int list) (adversario: int list) =
    Available jogador adversario
    |> List.maxBy (fun m -> placar (m::jogador) adversario)
