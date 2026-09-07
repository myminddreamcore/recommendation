using Avalonia.Controls;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using System.Net;using System.Linq;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Net.Http;
using RecommendationsAvalonia.Models;

using System.IO;
namespace RecommendationsAvalonia;

public partial class Podborka : Window
{
    public Podborka(User user)
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
    public int current = 0;
    public List<PodborDTO> d = new List<PodborDTO>();
    public async void Load()
    {
        string url = "http://localhost:5015/Compilation/GetPodborka";
        var response = await client.GetAsync(url);
        if(response.IsSuccessStatusCode)
        {
            d = JsonConvert.DeserializeObject<List<PodborDTO>>(await response.Content.ReadAsStringAsync());
            
        }
        Load2();

    }
    public void Load2()
    {
        var item = d[current];
        name.Text = item.Name;
        col.Text = item.users.Count.ToString();
        date.Text = item.Date.ToString();
        dg.ItemsSource = item.users;
    }
    public HttpClient client = new HttpClient();
    private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(newname.Text!="")
        {
            string url = $"http://localhost:5015/Compilation/Rename/{d[current].Id}/{newname.Text}";
            var response = await client.PutAsync(url, null);
            if(response.IsSuccessStatusCode)
            {
                await MessageBoxManager.GetMessageBoxStandard("", "успешно", ButtonEnum.Ok).ShowAsync();
                
            }
            Load();
        }
    }
    private void Button_Click_1(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(current>0){
        current--;
        Load2();
        }
    }
    private void Button_Click_2(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if(current<d.Count-1){
        current++;
        Load2();
        }
    }
   private async void Button_Click_3(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string url = $"http://localhost:5015/Compilation/Delete/{d[current].Id}";
            var response = await client.PutAsync(url, null);
            if(response.IsSuccessStatusCode)
            {
                await MessageBoxManager.GetMessageBoxStandard("", "успешно", ButtonEnum.Ok).ShowAsync();
                
            }
            current=0;
            Load();
    }
     
}