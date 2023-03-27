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

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;

while (true)
{
    Console.WriteLine("Что спарсить?\n1. Волейбол\n2. Футбол\n3. Баскетбол\n4. Теннис");
    if (int.TryParse(Console.ReadLine(), out var select))
    {
        Console.Clear();
        switch (select)
        {
            case 1:
                await Volleyball();
                Console.WriteLine("Закончили");
                continue;

            case 2:
                await Football();
                Console.WriteLine("Закончили");
                continue;

            case 3:
                await Basketball();
                Console.WriteLine("Закончили");
                continue;

            case 4:
                await Tennis();
                Console.WriteLine("Закончили");
                continue;

            default:
                Console.WriteLine("Неверно введен выбор, попробуйте еще раз");
                continue;
        }
    }
    else
    {
        Console.Clear();
        Console.WriteLine("Используйте только цифры");
    }
}

async Task Volleyball()
{
    var listOfMatch = new List<MatchInfo>();
    var options = new ChromeOptions();
    var service = ChromeDriverService.CreateDefaultService();
    service.HideCommandPromptWindow = true;
    options.AddArgument("no-sandbox");
    options.AddArgument("remote-debugging-port=0");
    options.AddArgument("disable-extensions");

    if (File.Exists("proxy.txt"))
    {
        var proxyData = (await File.ReadAllTextAsync("proxy.txt")).Split(':');
        //proxyData[0] - type
        //proxyData[1] - host
        //proxyData[2] - port
        options.AddArgument($"--proxy-server={proxyData[0]}://{proxyData[1]}:{proxyData[2]}");
    }

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
    {
        var element = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

        if (element != null && element.GetAttribute("title").Contains("Подробности матча!"))
        {
            var tmpCurrentGame = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div"));
            var tmpTime = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]"));
            var tmpScoreHome = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[5]"));
            var tmpScoreAway = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[6]"));

            if (tmpTime is null)
                continue;

            var tmpScoreOddsLose = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 7 : 6)}]/span"));
            var tmpScoreOddsWin = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 8 : 7)}]/span"));
            if (tmpScoreOddsLose is null || tmpScoreOddsWin is null)
                continue;

            var tmpNameTeam1 = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));
            var tmpNameTeam2 = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[4]"));
            var tmpId = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

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
            var tmpTtleMatchName = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div/span[2]"));
            NameLeague = (tmpTtleMatchName is null) ? string.Empty : tmpTtleMatchName.Text;

            var openMatches = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));

            if (openMatches != null && openMatches.Enabled)
                openMatches.Click();
        }
    }

    var html = GetMyTable(listOfMatch, x => $"{x.CurrentGame}|{x.Time}|{x.Name}|{x.NameTeam1}|{x.NameTeam2}|{x.ScoreHome}|{x.ScoreAway}|{x.OddsLose}|{x.OddsWin}|{x.OddsDraw}|{x.Link}");
    await File.WriteAllTextAsync("output.html", html);
    chrome.Close();
}

async Task Basketball()
{
    var listOfMatch = new List<MatchInfo>();
    var options = new ChromeOptions();
    var service = ChromeDriverService.CreateDefaultService();
    service.HideCommandPromptWindow = true;
    options.AddArgument("no-sandbox");
    options.AddArgument("remote-debugging-port=0");
    options.AddArgument("disable-extensions");

    if (File.Exists("proxy.txt"))
    {
        var proxyData = (await File.ReadAllTextAsync("proxy.txt")).Split(':');
        //proxyData[0] - type
        //proxyData[1] - host
        //proxyData[2] - port
        options.AddArgument($"--proxy-server={proxyData[0]}://{proxyData[1]}:{proxyData[2]}");
    }

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
    {
        var element = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

        if (element != null && element.GetAttribute("title").Contains("Подробности матча!"))
        {
            var tmpCurrentGame = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div"));
            var tmpTime = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]"));
            var tmpScoreHome = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[5]"));
            var tmpScoreAway = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[6]"));

            if (tmpTime is null)
                continue;

            var tmpScoreOddsLose = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 7 : 6)}]/span"));
            var tmpScoreOddsWin = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 8 : 7)}]/span"));
            if (tmpScoreOddsLose is null || tmpScoreOddsWin is null)
                continue;

            var tmpNameTeam1 = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));
            var tmpNameTeam2 = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[4]"));
            var tmpId = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

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
            var tmpTtleMatchName = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div/span[2]"));
            NameLeague = (tmpTtleMatchName is null) ? string.Empty : tmpTtleMatchName.Text;

            var openMatches = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));

            if (openMatches != null && openMatches.Enabled)
                openMatches.Click();
        }
    }

    var html = GetMyTable(listOfMatch, x => $"{x.CurrentGame}|{x.Time}|{x.Name}|{x.NameTeam1}|{x.NameTeam2}|{x.ScoreHome}|{x.ScoreAway}|{x.OddsLose}|{x.OddsWin}|{x.OddsDraw}|{x.Link}");
    await File.WriteAllTextAsync("output.html", html);
    chrome.Close();
}

async Task Tennis()
{
    var listOfMatch = new List<MatchInfo>();
    var options = new ChromeOptions();
    var service = ChromeDriverService.CreateDefaultService();
    service.HideCommandPromptWindow = true;
    options.AddArgument("no-sandbox");
    options.AddArgument("remote-debugging-port=0");
    options.AddArgument("disable-extensions");

    if (File.Exists("proxy.txt"))
    {
        var proxyData = (await File.ReadAllTextAsync("proxy.txt")).Split(':');
        //proxyData[0] - type
        //proxyData[1] - host
        //proxyData[2] - port
        options.AddArgument($"--proxy-server={proxyData[0]}://{proxyData[1]}:{proxyData[2]}");
    }

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
        var element = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

        if (element != null && element.GetAttribute("title").Contains("Подробности матча!"))
        {
            var tmpCurrentGame = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div"));
            var tmpTime = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]"));
            var tmpScoreHome = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[5]"));
            var tmpScoreAway = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[6]"));

            if (tmpTime is null)
                continue;

            var tmpScoreOddsLose = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 7 : 6)}]/span"));
            var tmpScoreOddsWin = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 8 : 7)}]/span"));
            if (tmpScoreOddsLose is null || tmpScoreOddsWin is null)
                continue;

            var tmpNameTeam1 = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));
            var tmpNameTeam2 = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[4]"));
            var tmpId = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

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
            var tmpTtleMatchName = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div/span[2]"));
            NameLeague = (tmpTtleMatchName is null) ? string.Empty : tmpTtleMatchName.Text;

            var openMatches = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));

            if (openMatches != null && openMatches.Enabled)
                openMatches.Click();
        }
    }

    var html = GetMyTable(listOfMatch, x => $"{x.CurrentGame}|{x.Time}|{x.Name}|{x.NameTeam1}|{x.NameTeam2}|{x.ScoreHome}|{x.ScoreAway}|{x.OddsLose}|{x.OddsWin}|{x.OddsDraw}|{x.Link}");
    await File.WriteAllTextAsync("output.html", html);
    chrome.Close();
}

async Task Football()
{
    var listOfMatch = new List<MatchInfo>();
    var options = new ChromeOptions();
    var service = ChromeDriverService.CreateDefaultService();
    service.HideCommandPromptWindow = true;
    options.AddArgument("no-sandbox");
    options.AddArgument("remote-debugging-port=0");
    options.AddArgument("disable-extensions");

    if (File.Exists("proxy.txt"))
    {
        var proxyData = (await File.ReadAllTextAsync("proxy.txt")).Split(':');
        //proxyData[0] - type
        //proxyData[1] - host
        //proxyData[2] - port
        options.AddArgument($"--proxy-server={proxyData[0]}://{proxyData[1]}:{proxyData[2]}");
    }

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
        var element = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

        if (element != null && element.GetAttribute("title").Contains("Подробности матча!"))
        {
            var tmpCurrentGame = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div"));
            var tmpTime = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]"));
            var tmpScoreHome = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[5]"));
            var tmpScoreAway = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[6]"));

            if (tmpTime is null)
                continue;

            var tmpScoreOddsLose = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 7 : 6)}]/span"));
            var tmpScoreOddsDraw = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 8 : 7)}]/span"));
            var tmpScoreOddsWin = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[{(tmpCurrentGame != null ? 9 : 8)}]/span"));
            if (tmpScoreOddsLose is null || tmpScoreOddsDraw is null || tmpScoreOddsWin is null)
                continue;

            var tmpNameTeam1 = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));
            var tmpNameTeam2 = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[4]"));
            var tmpId = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]"));

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
            var tmpTtleMatchName = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[2]/div/span[2]"));
            NameLeague = (tmpTtleMatchName is null) ? string.Empty : tmpTtleMatchName.Text;

            var openMatches = await FindElement(chrome, By.XPath($"/html/body/div[4]/div[1]/div/div/main/div[4]/div[2]/div/section/div/div/div[{i}]/div[3]"));

            if (openMatches != null && openMatches.Enabled)
                openMatches.Click();
        }
    }

    var html = GetMyTable(listOfMatch, x => $"{x.CurrentGame}|{x.Time}|{x.Name}|{x.NameTeam1}|{x.NameTeam2}|{x.ScoreHome}|{x.ScoreAway}|{x.OddsLose}|{x.OddsWin}|{x.OddsDraw}|{x.Link}");
    await File.WriteAllTextAsync("output.html", html);
    chrome.Close();
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
/*
async Task<string[]> GetLinks(string question, int page, string country, ChromeDriver driver)
{
    try
    {
        List<string> links = new List<string>();
        driver.Navigate().GoToUrl(@$"https://www.google.com/search?q={question}&start={page * 100}&num=100&gl={country}");
        foreach (WebElement link in driver.FindElements(By.XPath("//a[@href]")))
        {
            try
            {
                string hrefValue = link.GetAttribute("href");

                if (hrefValue[0] != '/' && !hrefValue.ToLower().Contains("google") && (hrefValue.ToLower().Contains("https://") || hrefValue.ToLower().Contains("http://") || hrefValue.ToLower().Contains("www.")))
                    if (!ExistsHost(hrefValue))
                        links.Add(hrefValue);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        if (!driver.PageSource.Contains("display:block;margin-left:53px"))
            links.Add("end");

        return links.ToArray();

        bool ExistsHost(string host)
        {
            foreach (var link in links)
                if (new Uri(link).Host.ToLower() == new Uri(host).Host.ToLower())
                    return true;

            return false;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
        return new string[] { };
    }
}

async Task<string> GetAsync(string url, int timeout = 10)
{
    try
    {
        HttpClient request = new HttpClient();

        request.Timeout = TimeSpan.FromSeconds(timeout);

        request.DefaultRequestHeaders.UserAgent.ParseAdd(@"Mozilla/5.0 (Windows; Windows NT 6.1) AppleWebKit/534.23 (KHTML, like Gecko) Chrome/11.0.686.3 Safari/534.23");

        HttpResponseMessage response = await request.GetAsync(url);

        //Bypass UTF8 error encoding
        byte[] buf = await response.Content.ReadAsByteArrayAsync();
        return Encoding.UTF8.GetString(buf);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.Message);
    }

    return string.Empty;
}*/