using System;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace ConsulClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var consul = new Consul.ConsulClient(c =>
            {
                c.Address = new Uri("http://127.0.0.1:8500");
            }))
            {
                //取在Consul注册的全部服务
                //var services = consul.Agent.Services().Result.Response;
                //foreach (var s in services.Values)
                //{
                //    Console.WriteLine($"ID={s.ID},Service={s.Service},Addr={s.Address},Port={s.Port}");

                //}

                //取出全部的DemoService服务
                var services = consul.Agent.Services().Result.Response.Values.Where(p => p.Service.Equals("DemoService", StringComparison.OrdinalIgnoreCase));
                //客户端负载均衡，随机选出一台服务
                Random rand = new Random();
                var index = rand.Next(services.Count());
                var s = services.ElementAt(index);
                Console.WriteLine($"Index={index},ID={s.ID},Service={s.Service},Addr={s.Address},Port={s.Port}");

                //向服务发送请求
                using (var httpClient = new HttpClient())
                using (var httpContent = new StringContent("{FirstName:'Alex',LastName:'Wang'}", Encoding.UTF8, "application/json"))
                {
                    var result = httpClient.PostAsync($"http://{s.Address}:{s.Port}/api/Values", httpContent);
                    Console.WriteLine($"调用{s.Service}，状态：{result.Result.StatusCode}");
                }

            }

            Console.ReadKey();
        }
    }
}
