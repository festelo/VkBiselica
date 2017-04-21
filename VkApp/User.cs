using System;
using System.Collections.Generic;
using System.Linq;
using VkNet;
using VkNet.Model.RequestParams;

namespace VkApp
{
    public class User
    {
        public bool IsPlaying => !_game?.Closed ?? false;
        public long Id;
        public List<string> MessageHistory = new List<string>();
        public HashSet<string> Words = new HashSet<string> { "Говнокод", "Тест", "ПитонГовно", "Ассемблер"};
        private readonly Random _rnd = new Random();
        private Game _game;
        public User(long id)
        {
            Id = id;
        }

        public void OnMessageReceived(VkApi api, Message msg)
        {
            var retMessage = "Чтобы начать игру напишите 'Старт'";

            if (!IsPlaying && string.Compare(msg.Text, "Старт", StringComparison.OrdinalIgnoreCase) == 0)
            {
                _game = new Game(Words.ToArray()[_rnd.Next(Words.Count)]);
                retMessage = $"Игра началась, напишите 'Стоп' чтобы закончить её\nСлово: {_game.HiddenWord}";
            }
            else if (IsPlaying && string.Compare(msg.Text, "Стоп", StringComparison.OrdinalIgnoreCase) == 0)
            {
                _game.Close();
                retMessage = "Игра закончена";
            }
            else if (IsPlaying)
            {
                var c = _game.OpenWord(msg.Text.Trim());
                if (_game.Closed)
                {
                    switch (_game.Status)
                    {
                        case GameStatus.Win:
                            retMessage = $"Вы выиграли! Слово {_game.Word}";
                            break;
                        case GameStatus.Lose:
                            retMessage = $"Вы проиграли! Слово {_game.Word}";
                            break;
                    }
                }
                else
                    retMessage =
                        $"Ваша строка была в слове {c} раз(а)\nСлово: {_game.HiddenWord}\nПопыток осталось: {_game.Tryes}";

            }
            else if (string.Compare(msg.Text.Split(' ')[0], "Добавить", StringComparison.OrdinalIgnoreCase) == 0 )
            {
                try
                {
                    var word = msg.Text.Split(' ')[1];
                    Words.Add(word);
                    retMessage = $"Слово '{word}' добавлено";
                }
                catch
                {
                    retMessage = $"При добавлении произошла ошибка";
                }
            }

            MessageHistory.Add(msg.Text);
            api.Messages.Send(new MessagesSendParams()
            {
                Message = retMessage,
                PeerId = Id
            });
        }
    }
}
