using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Waterfall
{
    public class InfoHelper {
        public class Weather
        {
            public string Qiwen { get; set; }
            public string code { get; set; }
            public string KongQiZhiLiang { get; set; }
            public string MiaoShu { get; set; }
            public string TiGan { get; set; }
            public string FenSu { get; set; }
            public string NenJianDu { get; set; }
            public List<WeatherByDay> Data { get; set; }

        }
        public class WeatherByDay
        {
            public string Date { set; get; }
            public string code { set; get; }
            public string QiWen { set; get; }
        }
    }
    public class HttpHelper {
        public static async Task<string> GetWebAsync(string url, Encoding e = null)
        {
            if (e == null)
                e = Encoding.UTF8;
            HttpWebRequest hwr = (HttpWebRequest)WebRequest.Create(url);
            hwr.Timeout = 20000;
            var o = await hwr.GetResponseAsync();
            StreamReader sr = new StreamReader(o.GetResponseStream(), e);
            var st = await sr.ReadToEndAsync();
            sr.Dispose();
            return st;
        }
    }
    public class WeatherLib
    {
        public static async Task<InfoHelper.Weather> GetWeatherAsync(string i)
        {
            InfoHelper.Weather dt = new InfoHelper.Weather();
            //空气质量
            var oj= JObject.Parse(await HttpHelper.GetWebAsync($"https://free-api.heweather.net/s6/air/now?parameters&location={Uri.EscapeUriString(i)}&key=f97e6a6ad4cd49babd0538747c86b88d"));
            dt.KongQiZhiLiang = oj["HeWeather6"][0]["air_now_city"]["qlty"]+"  "+ oj["HeWeather6"][0]["air_now_city"]["aqi"];
            //实况天气
            JObject obj = JObject.Parse(await HttpHelper.GetWebAsync($"https://free-api.heweather.net/s6/weather/now?parameters&location={Uri.EscapeUriString(i)}&key=f97e6a6ad4cd49babd0538747c86b88d"));
            dt.TiGan = "体感温度:" + obj["HeWeather6"][0]["now"]["fl"] + "℃";
            dt.FenSu= obj["HeWeather6"][0]["now"]["wind_dir"] + "    " + obj["HeWeather6"][0]["now"]["wind_sc"] + "级";
            dt.MiaoShu = obj["HeWeather6"][0]["now"]["cond_txt"].ToString();
            dt.code= obj["HeWeather6"][0]["now"]["cond_code"].ToString();
            dt.NenJianDu= "能见度:" + obj["HeWeather6"][0]["now"]["vis"];
            dt.Qiwen= obj["HeWeather6"][0]["now"]["tmp"] + "℃";
            //未来几天的预报
            JObject obj1 = JObject.Parse(await HttpHelper.GetWebAsync($"https://free-api.heweather.net/s6/weather/forecast?parameters&location={Uri.EscapeUriString(i)}&key=f97e6a6ad4cd49babd0538747c86b88d"));
            List<InfoHelper.WeatherByDay> ddt = new List<InfoHelper.WeatherByDay>();
            ddt.Add(new InfoHelper.WeatherByDay() {code=obj1["HeWeather6"][0]["daily_forecast"][0]["cond_code_d"].ToString(),
            Date= obj1["HeWeather6"][0]["daily_forecast"][0]["date"].ToString(),
            QiWen= obj1["HeWeather6"][0]["daily_forecast"][0]["tmp_max"] + "℃ -" + obj1["HeWeather6"][0]["daily_forecast"][0]["tmp_min"] + "℃"
            });
            for (int ix = 0; ix < obj1["HeWeather6"][0]["daily_forecast"].Count(); ix++) {
                ddt.Add(new InfoHelper.WeatherByDay()
                {
                    code=obj1["HeWeather6"][0]["daily_forecast"][ix]["cond_code_d"].ToString(),
                    Date = obj1["HeWeather6"][0]["daily_forecast"][ix]["date"].ToString(),
                    QiWen = obj1["HeWeather6"][0]["daily_forecast"][ix]["tmp_max"] + "℃ -" + obj1["HeWeather6"][0]["daily_forecast"][ix]["tmp_min"] + "°C"
                });
            }
            dt.Data = ddt;
            return dt;
        }
    }
}
