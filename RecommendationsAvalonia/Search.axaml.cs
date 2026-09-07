using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Net;
using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Net.Http;
using System.IO;
using RecommendationsAvalonia.Models;
namespace RecommendationsAvalonia;

public partial class Search : Window
{
    public Search(User user)
    {
        InitializeComponent();
        var fioBlock = this.FindControl<TextBlock>("fio");
        if (fioBlock != null)
        {
            fioBlock.Text = $"{user.NameUser} {user.SurnameUser} {user.PatronymicUser}";
        }
        
        var roleBlock = this.FindControl<TextBlock>("role");
        if (roleBlock != null)
        {
            roleBlock.Text = user.RoleUser;
        }
        Load();
    }

    public int pageSize = 20;
    public int currentPage = 1;
    public int pageCount = 1;
    public void RefreshDisplay()
    {
        var currentlist = finder;
        int total = currentlist.Count;
        pageCount = (int)Math.Ceiling(total / (double)pageSize);
        currentPage = Math.Clamp(currentPage, 1, pageCount > 0 ? pageCount : 1);
        List<FinderrDTO> paggedList = currentlist.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        dg.ItemsSource = paggedList;
    }
    List<string> skills = new List<string>();
    List<string> skillsvybor = new List<string>();
    public async void Load()
    {
        string url = "http://localhost:5015/Skill/skills";
        var response = await client.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            skills = JsonConvert.DeserializeObject<List<string>>(await response.Content.ReadAsStringAsync());

        }

    }
    public HttpClient client = new HttpClient();
    private void Button_Click_4(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string s = skills.FirstOrDefault(x => x.ToLower().Contains(skillsn.Text.ToLower()));
        skillsvybor.Add(s);
        skillname.Text = string.Join(", ", skillsvybor);

    }
    List<FinderrDTO> finder = new List<FinderrDTO>();
    private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string url = "http://localhost:5015/User/users/find";

        Find f = new Find()
        {
            skills = skillsvybor,
            UrovenOt = urovenot.Text != null ? Convert.ToInt32(urovenot.Text) : null,
            UrovenODo = urovendo.Text != null ? Convert.ToInt32(urovendo.Text) : null,
            OpytOt = opytot.Text != null ? Convert.ToInt32(opytot.Text) : null,
            OpytDo = opytdo.Text != null ? Convert.ToInt32(opytdo.Text) : null,
            RatingOt = ratingot.Text != null ? Convert.ToInt32(ratingot.Text) : null,
            ratingDo = ratingdo.Text != null ? Convert.ToInt32(ratingdo.Text) : null,
        };

        // await MessageBoxManager.GetMessageBoxStandard("", f.RatingOt.ToString(), ButtonEnum.Ok).ShowAsync();
        var json = JsonConvert.SerializeObject(f);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(url, content);
        Console.WriteLine(json);
        if (response.IsSuccessStatusCode)
        {
            string jsonString = await response.Content.ReadAsStringAsync();
            jsonString = jsonString.Replace(":null", ":0");
            jsonString = jsonString.Replace(":null", ":\"\"");

            finder = JsonConvert.DeserializeObject<List<FinderrDTO>>(jsonString);
            RefreshDisplay();
        }
        else
        {

            await MessageBoxManager.GetMessageBoxStandard("уведомление", "не найдено", ButtonEnum.Ok).ShowAsync();
        }
    }
    private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        dg.ItemsSource = null;
    }

    private void Button_Click_2(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {

    }
    private async void Button_Click_3(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string path = $"/home/user/export/{DateTime.Now}.csv";
        using (var writer = new StreamWriter(path, false, Encoding.UTF8))
        {
            writer.WriteLine("номер,имя,навыки,рейтинг");
            foreach (var item in finder)
            {
                writer.WriteLine($"{item.Id},{item.Name},{item.Skills},{item.Rating}");
            }
        }
        await MessageBoxManager.GetMessageBoxStandard("уведомление", "успешно", ButtonEnum.Ok).ShowAsync();
    }
    private void cb_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (cb.SelectedItem == null)
        {
            return;
        }
        else
        {
            if (cb.SelectedIndex == 0)
            {
                pageSize = 25;
                RefreshDisplay();
            }
            if (cb.SelectedIndex == 1)
            {
                pageSize = 50;
                RefreshDisplay();
            }
            if (cb.SelectedIndex == 2)
            {
                pageSize = 100;
                RefreshDisplay();
            }
        }
    }
    private void Button_Click_5(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        currentPage++;
        RefreshDisplay();
    }
    private void Button_Click_6(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        currentPage--;
        RefreshDisplay();
    }
    private void Button_Click_7(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (dg.SelectedItem is FinderrDTO f)
        {
            Card d = new Card(f.Id);
            d.Show();
        }
    }
    

    private object? GetProp(object obj, string prop)
        => obj.GetType().GetProperty(prop)?.GetValue(obj);

    private void MyDataGrid_Sorting(object? sender, DataGridColumnEventArgs e)
    {
         var dg = (DataGrid)sender!;
        var header = e.Column.Header?.ToString()!;

        var items = dg.ItemsSource!.Cast<object>().ToList();
        var sorted = items.OrderBy(x => GetProp(x, header)).ToList();

        dg.ItemsSource = sorted;
        e.Handled = true;
    }
}