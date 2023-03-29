using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

var tApi = new TelegramBotClient("6251234348:AAHZrzsngH0E_uoXfztBBpW8HCeFleEDlYA");
//Console.Beep(400, 2_000);
//await tApi.SendTextMessageAsync(new Telegram.Bot.Types.ChatId(797217283), "test");

var tasks = new List<Task>();

tasks.Add(Task.Run(async () =>
{
    while (true)
    {
        var allTables = new List<MatchInfo>();
        var time = DateTime.Now;

        var volleyball = await Volleyball();
        var basketball = await Basketball();
        var tennis = await Tennis();
        var football = await Football();

        if (time.Hour == 0 && time.Minute == 0)
        {
            await UpdateTableInfo(volleyball, basketball, tennis, football);
            await Task.Delay(120_000);
        }
        else if (time.Hour == 10 && time.Minute == 0)
        {
            await UpdateTableInfo(volleyball, basketball, tennis, football);
            await Task.Delay(120_000);
        }
        else if (time.Hour == 18 && time.Minute == 0)
        {
            await UpdateTableInfo(volleyball, basketball, tennis, football);
            await Task.Delay(120_000);
        }

        foreach (var match in volleyball)
        {
            if (TimeSpan.TryParse(match.Time, out _))
                if (match.Time.Contains(":"))
                    continue;

            if (!int.TryParse(new Regex(@"\d+").Match(match.Time).Value, out _))
                continue;

            var set = int.Parse(new Regex(@"\d+").Match(match.Time).Value);
            var allScores = await GetScore(match.Link);

            var scoreHome = allScores[0];
            var scoreAway = allScores[1];

            //OddsLose - Home
            //OddsWin - Away

            if (set == 2)
            {
                if (match.OddsLose < match.OddsWin)
                {
                    if (scoreHome == 2 && scoreAway == 0)
                    {
                        Console.Beep(300, 5_000);
                        await tApi.SendTextMessageAsync(new Telegram.Bot.Types.ChatId(797217283), $"Волейбол: побеждает команда {match.NameTeam1} с меньшим коэф. {match.OddsLose}");
                    }
                }
                else if (match.OddsWin < match.OddsLose)
                {
                    if (scoreHome == 0 && scoreAway == 2)
                    {
                        Console.Beep(300, 5_000);
                        await tApi.SendTextMessageAsync(new Telegram.Bot.Types.ChatId(797217283), $"Волейбол: побеждает команда {match.NameTeam2} с меньшим коэф. {match.OddsWin}");
                    }
                }
            }
            else if (set == 3)
            {
                if (match.OddsLose < match.OddsWin)
                {
                    if (scoreHome == 1 && scoreAway == 2)
                    {
                        Console.Beep(300, 5_000);
                        await tApi.SendTextMessageAsync(new Telegram.Bot.Types.ChatId(797217283), $"Волейбол: побеждает команда {match.NameTeam1} с меньшим коэф. {match.OddsLose}");
                    }
                }
                else if (match.OddsWin < match.OddsLose)
                {
                    if (scoreHome == 2 && scoreAway == 1)
                    {
                        Console.Beep(300, 5_000);
                        await tApi.SendTextMessageAsync(new Telegram.Bot.Types.ChatId(797217283), $"Волейбол: побеждает команда {match.NameTeam2} с меньшим коэф. {match.OddsWin}");
                    }
                }
            }
        }

        foreach (var match in tennis)
        {
            if (TimeSpan.TryParse(match.Time, out _))
                if (match.Time.Contains(":"))
                    continue;

            if (!int.TryParse(new Regex(@"\d+").Match(match.Time).Value, out _))
                continue;

            var set = int.Parse(new Regex(@"\d+").Match(match.Time).Value);
            var allScores = await GetScore(match.Link);

            var scoreHome = allScores[0];
            var scoreAway = allScores[1];

            //OddsLose - Home
            //OddsWin - Away

            if (set >= 1)
            {
                if (match.OddsLose < match.OddsWin)
                {
                    if (scoreHome == 1 && scoreAway == 0)
                    {
                        Console.Beep(300, 5_000);
                        await tApi.SendTextMessageAsync(new Telegram.Bot.Types.ChatId(797217283), $"Тенис: побеждает команда {match.NameTeam1} с меньшим коэф. {match.OddsLose}");
                    }
                }
                else if (match.OddsWin < match.OddsLose)
                {
                    if (scoreHome == 0 && scoreAway == 1)
                    {
                        Console.Beep(300, 5_000);
                        await tApi.SendTextMessageAsync(new Telegram.Bot.Types.ChatId(797217283), $"Тенис: побеждает команда {match.NameTeam2} с меньшим коэф. {match.OddsWin}");
                    }
                }
            }
        }

        foreach (var match in football)
        {
            if (TimeSpan.TryParse(match.Time, out _))
                if (match.Time.Contains(":"))
                    continue;

            if (!int.TryParse(new Regex(@"\d+").Match(match.Time).Value, out _))
                continue;

            var set = int.Parse(new Regex(@"\d+").Match(match.Time).Value);
            var allScores = await GetScore(match.Link);

            var scoreHome = allScores[0];
            var scoreAway = allScores[1];

            //OddsLose - Home
            //OddsWin - Away

            if (set >= 69)
            {
                if (scoreHome == 0 && scoreAway == 0)
                {
                    Console.Beep(300, 5_000);
                    await tApi.SendTextMessageAsync(new Telegram.Bot.Types.ChatId(797217283), $"Футбол: 69-ая минута, ничья");
                }
                else
                {
                    if (match.OddsLose < match.OddsWin)
                    {
                        Console.Beep(300, 5_000);
                        await tApi.SendTextMessageAsync(new Telegram.Bot.Types.ChatId(797217283), $"Футбол: команда {match.NameTeam1} лидер проигрывает с меньшим коэф. {match.OddsLose}");
                    }
                    else if (match.OddsWin < match.OddsLose)
                    {
                        Console.Beep(300, 5_000);
                        await tApi.SendTextMessageAsync(new Telegram.Bot.Types.ChatId(797217283), $"Футбол: команда {match.NameTeam2} лидер проигрывает с меньшим коэф. {match.OddsWin}");
                    }
                }
            }
        }

        await Task.Delay(300_000);
    }
}));

Task.WaitAll(tasks.ToArray());

async Task<int[]> GetScore(string link)
{
    var listOfScore = new List<int>();
    var options = new ChromeOptions();
    var service = ChromeDriverService.CreateDefaultService();
    service.HideCommandPromptWindow = true;
    options.AddArgument("no-sandbox");
    options.AddArgument("remote-debugging-port=0");
    options.AddArgument("disable-extensions");

    var chrome = new ChromeDriver(service, options);

    chrome.Navigate().GoToUrl(link);

    await Task.Delay(3000);

    listOfScore.Add(int.Parse((await FindElement(chrome, By.XPath("//*[@id=\"detail\"]/div[5]/div[3]/div/div[1]/span[1]")))!.Text));
    listOfScore.Add(int.Parse((await FindElement(chrome, By.XPath("//*[@id=\"detail\"]/div[5]/div[3]/div/div[1]/span[3]")))!.Text));

    /*if (isHome)
        for (var x = 9; x <= 10; x++)
            for (var i = 4; i <= 8; i++)
            {
                var element = await FindElement(chrome, By.XPath($"//*[@id=\"detail\"]/div[{x}]/div/div[2]/div[{i}]"));

                if (element != null && int.TryParse(element.Text, out int score))
                    listOfScore.Add(score);
            }
    else
        for (var x = 9; x <= 10; x++)
            for (var i = 12; i <= 16; i++)
            {
                var element = await FindElement(chrome, By.XPath($"//*[@id=\"detail\"]/div[{x}]/div/div[2]/div[{i}]"));

                if (element != null && int.TryParse(element.Text, out int score))
                    listOfScore.Add(score);
            }*/

    chrome.Close();

    return listOfScore.ToArray();
}

async Task UpdateTableInfo(MatchInfo[] volleyball, MatchInfo[] basketball, MatchInfo[] tennis, MatchInfo[] football)
{
    var allTables = new List<MatchInfo>();

    allTables.AddRange(volleyball.Where(x => x.OddsWin < 1.41f || x.OddsLose < 1.41f));
    allTables.AddRange(basketball.Where(x => x.OddsWin < 1.5f || x.OddsLose < 1.5f));
    allTables.AddRange(tennis.Where(x => x.OddsWin < 1.36f || x.OddsLose < 1.36f));
    allTables.AddRange(football.Where(x => x.OddsWin < 1.41f || x.OddsDraw < 1.41f || x.OddsLose < 1.41f));

    var html = GetMyTable(allTables, x => $"{x.CurrentGame}|{x.Time}|{x.Name}|{x.NameTeam1}|{x.NameTeam2}|{x.ScoreHome}|{x.ScoreAway}|{x.OddsLose}|{x.OddsWin}|{x.OddsDraw}|{x.Link}");
    await File.WriteAllTextAsync("output.html", html);
}

async Task<MatchInfo[]> Volleyball()
{
    var listOfMatch = new List<MatchInfo>();
    var options = new ChromeOptions();
    var service = ChromeDriverService.CreateDefaultService();
    service.HideCommandPromptWindow = true;
    options.AddArgument("no-sandbox");
    options.AddArgument("remote-debugging-port=0");
    options.AddArgument("disable-extensions");

    /*if (File.Exists("proxy.txt"))
    {
        var proxyData = (await File.ReadAllTextAsync("proxy.txt")).Split(':');
        //proxyData[0] - type
        //proxyData[1] - host
        //proxyData[2] - port
        options.AddArgument($"--proxy-server={proxyData[0]}://{proxyData[1]}:{proxyData[2]}");
    }*/

    //options.AddArgument("headless");

    var chrome = new ChromeDriver(service, options);

    chrome.Navigate().GoToUrl("https://www.flashscorekz.com/volleyball/");

    while (!IsElementExist(chrome, By.XPath("//*[@id=\"onetrust-accept-btn-handler\"]")))
        await Task.Delay(500);

    chrome.FindElement(By.XPath("//*[@id=\"onetrust-accept-btn-handler\"]")).Click();

    while (!IsElementExist(chrome, By.XPath("//*[@id=\"live-table\"]/div[1]/div[1]/div[3]")))
        await Task.Delay(500);

    chrome.FindElement(By.XPath("//*[@id=\"live-table\"]/div[1]/div[1]/div[3]")).Click();
    await Task.Delay(3000);

    var NameLeague = string.Empty;

    for (var i = 0; i <= 200; i++)
    {                                                     //html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[2]
        var element = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

        if (element != null && element.GetAttribute("title").Contains("Подробности матча!"))
        {
            var tmpCurrentGame = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div"));
            var tmpTime = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]"));
            var tmpScoreHome = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[5]"));
            var tmpScoreAway = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[6]"));

            if (tmpTime is null)
                continue;

            var tmpScoreOddsLose = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 7 : 6)}]/span"));
            var tmpScoreOddsWin = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 8 : 7)}]/span"));
            if (tmpScoreOddsLose is null || tmpScoreOddsWin is null)
                continue;

            var tmpNameTeam1 = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));
            var tmpNameTeam2 = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[4]"));
            var tmpId = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

            var Time = (tmpTime is null) ? string.Empty : tmpTime.Text;
            var NameTeam1 = (tmpNameTeam1 is null) ? string.Empty : tmpNameTeam1.Text;
            var NameTeam2 = (tmpNameTeam2 is null) ? string.Empty : tmpNameTeam2.Text;
            var OddsLose = (tmpScoreOddsLose is null) ? -1f : float.Parse(tmpScoreOddsLose.Text.Replace('.', ','));
            var OddsWin = (tmpScoreOddsWin is null) ? -1f : float.Parse(tmpScoreOddsWin.Text.Replace('.', ','));
            var Id = (tmpId is null) ? string.Empty : tmpId.GetAttribute("id").Replace("g_12_", "");
            var CurrentGame = (tmpCurrentGame is null) ? string.Empty : tmpCurrentGame.Text;
            var ScoreHome = (tmpScoreHome is null) ? string.Empty : tmpScoreHome.Text;
            var ScoreAway = (tmpScoreAway is null) ? string.Empty : tmpScoreAway.Text;

            listOfMatch.Add(new MatchInfo()
            {
                Name = NameLeague,
                Time = Time,
                NameTeam1 = NameTeam1,
                NameTeam2 = NameTeam2,
                OddsLose = OddsLose,
                OddsDraw = 0,
                OddsWin = OddsWin,
                Link = $"https://www.flashscorekz.com/match/{Id}",
                CurrentGame = CurrentGame,
                ScoreHome = (ScoreHome == "-") ? 0 : int.Parse(ScoreHome),
                ScoreAway = (ScoreHome == "-") ? 0 : int.Parse(ScoreAway),
            });
        }
        else if (element != null)
        {
            var tmpTtleMatchName = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div/span[2]"));
            NameLeague = (tmpTtleMatchName is null) ? string.Empty : tmpTtleMatchName.Text;

            var openMatches = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));

            if (openMatches != null && openMatches.Enabled)
                openMatches.Click();
        }
    }

    chrome.Close();

    return listOfMatch.ToArray();
}

async Task<MatchInfo[]> Basketball()
{
    var listOfMatch = new List<MatchInfo>();
    var options = new ChromeOptions();
    var service = ChromeDriverService.CreateDefaultService();
    service.HideCommandPromptWindow = true;
    options.AddArgument("no-sandbox");
    options.AddArgument("remote-debugging-port=0");
    options.AddArgument("disable-extensions");

    /*if (File.Exists("proxy.txt"))
    {
        var proxyData = (await File.ReadAllTextAsync("proxy.txt")).Split(':');
        //proxyData[0] - type
        //proxyData[1] - host
        //proxyData[2] - port
        options.AddArgument($"--proxy-server={proxyData[0]}://{proxyData[1]}:{proxyData[2]}");
    }*/

    //options.AddArgument("headless");

    var chrome = new ChromeDriver(service, options);

    chrome.Navigate().GoToUrl("https://www.flashscorekz.com/basketball/");

    while (!IsElementExist(chrome, By.XPath("//*[@id=\"onetrust-accept-btn-handler\"]")))
        await Task.Delay(500);

    chrome.FindElement(By.XPath("//*[@id=\"onetrust-accept-btn-handler\"]")).Click();

    while (!IsElementExist(chrome, By.XPath("//*[@id=\"live-table\"]/div[1]/div[1]/div[3]")))
        await Task.Delay(500);

    chrome.FindElement(By.XPath("//*[@id=\"live-table\"]/div[1]/div[1]/div[3]")).Click();
    await Task.Delay(3000);

    var NameLeague = string.Empty;

    for (var i = 0; i <= 200; i++)
    {                                                     //html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[2]/div[6]/span
        var element = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

        if (element != null && element.GetAttribute("title").Contains("Подробности матча!"))
        {
            var tmpCurrentGame = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div"));
            var tmpTime = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]"));
            var tmpScoreHome = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[5]"));
            var tmpScoreAway = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[6]"));

            if (tmpTime is null)
                continue;

            var tmpScoreOddsLose = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 7 : 6)}]/span"));
            var tmpScoreOddsWin = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 8 : 7)}]/span"));
            if (tmpScoreOddsLose is null || tmpScoreOddsWin is null)
                continue;

            var tmpNameTeam1 = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));
            var tmpNameTeam2 = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[4]"));
            var tmpId = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

            var Time = (tmpTime is null) ? string.Empty : tmpTime.Text;
            var NameTeam1 = (tmpNameTeam1 is null) ? string.Empty : tmpNameTeam1.Text;
            var NameTeam2 = (tmpNameTeam2 is null) ? string.Empty : tmpNameTeam2.Text;
            var OddsLose = (tmpScoreOddsLose is null) ? -1f : float.Parse(tmpScoreOddsLose.Text.Replace('.', ','));
            var OddsWin = (tmpScoreOddsWin is null) ? -1f : float.Parse(tmpScoreOddsWin.Text.Replace('.', ','));
            var Id = (tmpId is null) ? string.Empty : tmpId.GetAttribute("id").Replace("g_1_", "");
            var CurrentGame = (tmpCurrentGame is null) ? string.Empty : tmpCurrentGame.Text;
            var ScoreHome = (tmpScoreHome is null) ? string.Empty : tmpScoreHome.Text;
            var ScoreAway = (tmpScoreAway is null) ? string.Empty : tmpScoreAway.Text;

            listOfMatch.Add(new MatchInfo()
            {
                Name = NameLeague,
                Time = Time,
                NameTeam1 = NameTeam1,
                NameTeam2 = NameTeam2,
                OddsLose = OddsLose,
                OddsDraw = 0,
                OddsWin = OddsWin,
                Link = $"https://www.flashscorekz.com/match/{Id}",
                CurrentGame = CurrentGame,
                ScoreHome = (ScoreHome == "-") ? 0 : int.Parse(ScoreHome),
                ScoreAway = (ScoreHome == "-") ? 0 : int.Parse(ScoreAway),
            });
        }
        else if (element != null)
        {
            var tmpTtleMatchName = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div/span[2]"));
            NameLeague = (tmpTtleMatchName is null) ? string.Empty : tmpTtleMatchName.Text;

            var openMatches = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));

            if (openMatches != null && openMatches.Enabled)
                openMatches.Click();
        }
    }

    chrome.Close();

    return listOfMatch.ToArray();
}

async Task<MatchInfo[]> Tennis()
{
    var listOfMatch = new List<MatchInfo>();
    var options = new ChromeOptions();
    var service = ChromeDriverService.CreateDefaultService();
    service.HideCommandPromptWindow = true;
    options.AddArgument("no-sandbox");
    options.AddArgument("remote-debugging-port=0");
    options.AddArgument("disable-extensions");

    /*if (File.Exists("proxy.txt"))
    {
        var proxyData = (await File.ReadAllTextAsync("proxy.txt")).Split(':');
        //proxyData[0] - type
        //proxyData[1] - host
        //proxyData[2] - port
        options.AddArgument($"--proxy-server={proxyData[0]}://{proxyData[1]}:{proxyData[2]}");
    }*/

    //options.AddArgument("headless");

    var chrome = new ChromeDriver(service, options);

    chrome.Navigate().GoToUrl("https://www.flashscorekz.com/tennis/");

    while (!IsElementExist(chrome, By.XPath("//*[@id=\"onetrust-accept-btn-handler\"]")))
        await Task.Delay(500);

    chrome.FindElement(By.XPath("//*[@id=\"onetrust-accept-btn-handler\"]")).Click();

    while (!IsElementExist(chrome, By.XPath("//*[@id=\"live-table\"]/div[1]/div[1]/div[3]")))
        await Task.Delay(500);

    chrome.FindElement(By.XPath("//*[@id=\"live-table\"]/div[1]/div[1]/div[3]")).Click();
    await Task.Delay(3000);

    var NameLeague = string.Empty;

    for (var i = 0; i <= 200; i++)
    {
        var element = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

        if (element != null && element.GetAttribute("title").Contains("Подробности матча!"))
        {
            var tmpCurrentGame = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div"));
            var tmpTime = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]"));
            var tmpScoreHome = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[5]"));
            var tmpScoreAway = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[6]"));

            if (tmpTime is null)
                continue;

            var tmpScoreOddsLose = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 7 : 6)}]/span"));
            var tmpScoreOddsWin = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 8 : 7)}]/span"));
            if (tmpScoreOddsLose is null || tmpScoreOddsWin is null)
                continue;

            var tmpNameTeam1 = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));
            var tmpNameTeam2 = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[4]"));
            var tmpId = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

            var Time = (tmpTime is null) ? string.Empty : tmpTime.Text;
            var NameTeam1 = (tmpNameTeam1 is null) ? string.Empty : tmpNameTeam1.Text;
            var NameTeam2 = (tmpNameTeam2 is null) ? string.Empty : tmpNameTeam2.Text;
            var OddsLose = (tmpScoreOddsLose is null) ? -1f : float.Parse(tmpScoreOddsLose.Text.Replace('.', ','));
            var OddsWin = (tmpScoreOddsWin is null) ? -1f : float.Parse(tmpScoreOddsWin.Text.Replace('.', ','));
            var Id = (tmpId is null) ? string.Empty : tmpId.GetAttribute("id").Replace("g_2_", "");
            var CurrentGame = (tmpCurrentGame is null) ? string.Empty : tmpCurrentGame.Text;
            var ScoreHome = (tmpScoreHome is null) ? string.Empty : tmpScoreHome.Text;
            var ScoreAway = (tmpScoreAway is null) ? string.Empty : tmpScoreAway.Text;

            listOfMatch.Add(new MatchInfo()
            {
                Name = NameLeague,
                Time = Time,
                NameTeam1 = NameTeam1,
                NameTeam2 = NameTeam2,
                OddsLose = OddsLose,
                OddsDraw = 0,
                OddsWin = OddsWin,
                Link = $"https://www.flashscorekz.com/match/{Id}",
                CurrentGame = CurrentGame,
                ScoreHome = (ScoreHome == "-") ? 0 : int.Parse(ScoreHome),
                ScoreAway = (ScoreHome == "-") ? 0 : int.Parse(ScoreAway),
            });
        }
        else if (element != null)
        {
            var tmpTtleMatchName = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div/span[2]"));
            NameLeague = (tmpTtleMatchName is null) ? string.Empty : tmpTtleMatchName.Text;

            var openMatches = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));

            if (openMatches != null && openMatches.Enabled)
                openMatches.Click();
        }
    }

    chrome.Close();

    return listOfMatch.ToArray();
}

async Task<MatchInfo[]> Football()
{
    var listOfMatch = new List<MatchInfo>();
    var options = new ChromeOptions();
    var service = ChromeDriverService.CreateDefaultService();
    service.HideCommandPromptWindow = true;
    options.AddArgument("no-sandbox");
    options.AddArgument("remote-debugging-port=0");
    options.AddArgument("disable-extensions");

    /*if (File.Exists("proxy.txt"))
    {
        var proxyData = (await File.ReadAllTextAsync("proxy.txt")).Split(':');
        //proxyData[0] - type
        //proxyData[1] - host
        //proxyData[2] - port
        options.AddArgument($"--proxy-server={proxyData[0]}://{proxyData[1]}:{proxyData[2]}");
    }*/

    //options.AddArgument("headless");

    var chrome = new ChromeDriver(service, options);

    chrome.Navigate().GoToUrl("https://www.flashscorekz.com/");

    while (!IsElementExist(chrome, By.XPath("//*[@id=\"onetrust-accept-btn-handler\"]")))
        await Task.Delay(500);

    chrome.FindElement(By.XPath("//*[@id=\"onetrust-accept-btn-handler\"]")).Click();

    while (!IsElementExist(chrome, By.XPath("//*[@id=\"live-table\"]/div[1]/div[1]/div[3]")))
        await Task.Delay(500);

    chrome.FindElement(By.XPath("//*[@id=\"live-table\"]/div[1]/div[1]/div[3]")).Click();
    await Task.Delay(3000);

    var NameLeague = string.Empty;

    for (var i = 0; i <= 200; i++)
    {
        var element = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

        if (element != null && element.GetAttribute("title").Contains("Подробности матча!"))
        {
            var tmpCurrentGame = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div"));
            var tmpTime = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]"));
            var tmpScoreHome = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[5]"));
            var tmpScoreAway = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[6]"));

            if (tmpTime is null)
                continue;

            var tmpScoreOddsLose = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 7 : 6)}]/span"));
            var tmpScoreOddsDraw = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 8 : 7)}]/span"));
            var tmpScoreOddsWin = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 9 : 8)}]/span"));
            if (tmpScoreOddsLose is null || tmpScoreOddsDraw is null || tmpScoreOddsWin is null)
                continue;

            var tmpNameTeam1 = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));
            var tmpNameTeam2 = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[4]"));
            var tmpId = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

            var Time = (tmpTime is null) ? string.Empty : tmpTime.Text;
            var NameTeam1 = (tmpNameTeam1 is null) ? string.Empty : tmpNameTeam1.Text;
            var NameTeam2 = (tmpNameTeam2 is null) ? string.Empty : tmpNameTeam2.Text;
            var OddsLose = (tmpScoreOddsLose is null) ? -1f : float.Parse(tmpScoreOddsLose.Text.Replace('.', ','));
            var OddDraw = (tmpScoreOddsDraw is null) ? -1f : float.Parse(tmpScoreOddsDraw.Text.Replace('.', ','));
            var OddsWin = (tmpScoreOddsWin is null) ? -1f : float.Parse(tmpScoreOddsWin.Text.Replace('.', ','));
            var Id = (tmpId is null) ? string.Empty : tmpId.GetAttribute("id").Replace("g_1_", "");
            var CurrentGame = (tmpCurrentGame is null) ? string.Empty : tmpCurrentGame.Text;
            var ScoreHome = (tmpScoreHome is null) ? string.Empty : tmpScoreHome.Text;
            var ScoreAway = (tmpScoreAway is null) ? string.Empty : tmpScoreAway.Text;

            listOfMatch.Add(new MatchInfo()
            {
                Name = NameLeague,
                Time = Time,
                NameTeam1 = NameTeam1,
                NameTeam2 = NameTeam2,
                OddsLose = OddsLose,
                OddsDraw = OddDraw,
                OddsWin = OddsWin,
                Link = $"https://www.flashscorekz.com/match/{Id}",
                CurrentGame = CurrentGame,
                ScoreHome = (ScoreHome == "-") ? 0 : int.Parse(ScoreHome),
                ScoreAway = (ScoreHome == "-") ? 0 : int.Parse(ScoreAway),
            });
        }
        else if (element != null)
        {
            var tmpTtleMatchName = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div/span[2]"));
            NameLeague = (tmpTtleMatchName is null) ? string.Empty : tmpTtleMatchName.Text;

            var openMatches = await FindElement(chrome, By.XPath($"/html/body/div[3]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));

            if (openMatches != null && openMatches.Enabled)
                openMatches.Click();
        }
    }

    chrome.Close();

    return listOfMatch.ToArray();
}

string GetMyTable<T>(IEnumerable<T> list, params Func<T, object>[] fxns)
{
    StringBuilder sb = new StringBuilder();
    sb.Append("<TABLE>\n");
    foreach (var item in list)
    {
        sb.Append("<TR>\n");
        foreach (var fxn in fxns)
        {
            sb.Append("<TD>");
            sb.Append(fxn(item));
            sb.Append("</TD>");
        }
        sb.Append("</TR>\n");
    }
    sb.Append("</TABLE>");

    return sb.ToString();
}

async Task<IWebElement?> FindElement(IWebDriver driver, By by, bool wait = false)
{
    while (!IsElementExist(driver, by) && wait)
        await Task.Delay(500);

    if (IsElementExist(driver, by))
        return driver.FindElement(by);
    else
        return null;
}

bool IsElementExist(IWebDriver driver, By by)
{
    try
    {
        var element = driver.FindElement(by);
        return element.Displayed;
    }
    catch (NoSuchElementException)
    {
        return false;
    }
}

struct MatchInfo
{
    public string Time;
    public string Name;
    public string NameTeam1;
    public string NameTeam2;
    public float OddsLose;
    public float OddsDraw;
    public float OddsWin;
    public string Link;

    public string CurrentGame;
    public int ScoreHome;
    public int ScoreAway;
}