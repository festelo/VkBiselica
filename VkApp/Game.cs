using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VkApp
{
    public enum GameStatus
    {
        Lose, Win, Playing, Closed
    }
    public class Game
    {
        public string Word { get; }
        public string HiddenWord { get; private set; }
        public int Tryes { get; private set; } = 5;
        public bool Closed { get; private set; }
        public GameStatus Status { get; private set; } = GameStatus.Playing;

        public Game(string word )
        {
            Word = word;
            var hiddenWordList = Enumerable.Repeat('*', Word.Length - 2).ToList();
            hiddenWordList.Insert(0, Word[0]);
            hiddenWordList.Add(Word.Last());
            HiddenWord = new string(hiddenWordList.ToArray());
        }

        public void Close()
        {
            Closed = true;
            Status = GameStatus.Closed;
        }

        public int OpenWord(string inp)
        {
            if(Closed)
                throw new Exception("Game closed");
            var counter = 0;
            var i = Word.IndexOf(inp, StringComparison.OrdinalIgnoreCase);
            while (i != -1)
            {
                counter++;
                HiddenWord = HiddenWord.ReplaceAt(i, inp.Length, new string(Word.Skip(i).Take(inp.Length).ToArray()));
                i = Word.IndexOf(inp, i+1, StringComparison.Ordinal);
            }
            if (counter == 0)
            {
                Tryes--;
                if (Tryes == 0)
                {
                    Closed = true;
                    Status = GameStatus.Lose;
                }
            }
            else if (Word == HiddenWord)
            {
                Closed = true;
                Status = GameStatus.Win;
            }
            return counter;
        }
    }
}
