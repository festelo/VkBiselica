using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace VkApp
{
    public class VkApp
    {
        private VkApi api = new VkApi();
        private Dictionary<long, User> users = new Dictionary<long, User>();
        
        public async Task AuthorizeAsync()
        {
            await api.AuthorizeAsync(new ApiAuthParams()
            {
                AccessToken = "9342df422c1ddb7bae79454e48bc15ae14d22fe6fe59b0bc9c93eea936afe0c704c02acf58b4884229356"
            });
        }

        public void Listen(CancellationToken? token = null)
        {
            var h = new HttpClient();
            var poll = api.Messages.GetLongPollServer();

            while (token == null || token.Value.IsCancellationRequested == false)
            {
                var uri = GetPollUri(poll);

                var getT = h.GetAsync(uri); getT.Wait();
                var responceT = getT.Result.Content.ReadAsStringAsync(); responceT.Wait();

                var responce = responceT.Result;
                dynamic results = JsonConvert.DeserializeObject<dynamic>(responce);


                poll.Ts = results.ts;
                var updates = results.updates;

                foreach (var u in updates)
                {
                    if (u[0] == 4 && ((int)u[2] & 2) == 0)
                    {
                        OnMessageReceived(new Message() { Text = u[6], UserId = u[3]});
                    }
                }
            }
        }


        private Uri GetPollUri(LongPollServerResponse poll)
        {
            return new Uri($"https://{poll.Server}?act=a_check&key={poll.Key}&ts={poll.Ts}&wait=25&version=1 ");
        }
        private void OnMessageReceived(Message msg)
        {
            var id = msg.UserId;
            if (!users.TryGetValue(id, out User u))
            {
                users.Add(id, u = new User(id));
                api.Messages.Send(new MessagesSendParams()
                {
                    Message = "Доброго дня желаю вам я. 'Старт' для начала напишите вы.\nСлово свое командою 'Добавить СЛОВО' вы можете, видно оно для вас только будет.",
                    PeerId = id
                });
            }
            else
                u.OnMessageReceived(api, msg);
        }
    }
}
