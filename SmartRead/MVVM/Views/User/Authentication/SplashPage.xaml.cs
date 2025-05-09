using Microsoft.Maui.Controls;
using System;

namespace SmartRead.MVVM.Views.User.Authentication
{
    public partial class SplashPage : ContentPage
    {
        public SplashPage()
        {
            InitializeComponent();

            // HTML con tu SVG inline (igual al que ya tenías)
            var html = @"
<!DOCTYPE html>
<html>
<head>
  <meta charset=""UTF-8"" />
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"" />
  <style>
    html, body {
      margin: 0;
      padding: 0;
      background: #000;
      overflow: hidden;
      height: 100%;
      width: 100%;
    }
  </style>
  <link href=""https://fonts.googleapis.com/css2?family=Bebas+Neue&display=swap"" rel=""stylesheet"">
</head>
<body>
  <svg width=""100%"" height=""100%"" viewBox=""0 0 1080 1920"" xmlns=""http://www.w3.org/2000/svg"">
    <!-- Fondo negro -->
    <rect width=""100%"" height=""100%"" fill=""#000""/>

    <!-- Definición de símbolo: banderita de España -->
    <symbol id=""flag-es"" viewBox=""0 0 6 4"">
      <rect width=""6"" height=""4"" fill=""#AA151B""/>
      <rect y=""1"" width=""6"" height=""2"" fill=""#F1BF00""/>
      <path d=""M0,0 Q1,1 2,0 T4,0 T6,0 L6,4 Q5,3 4,4 T2,4 T0,4 Z""
            fill-opacity=""0.2"">
        <animateTransform
          attributeName=""transform""
          type=""translate""
          values=""0,0;0.2,0;0,0""
          dur=""0.6s""
          repeatCount=""indefinite""/>
      </path>
    </symbol>

    <!-- Muchas banderitas desde todos los bordes -->
    <g id=""flags"">
      <!-- Desde arriba -->
      <use href=""#flag-es"" width=""60"" height=""40"" x=""10""   y=""-100"">
        <animateMotion dur=""5s"" repeatCount=""indefinite"" path=""M0,0 L0,2020""/>
      </use>
      <use href=""#flag-es"" width=""80"" height=""53"" x=""300""  y=""-150"">
        <animateMotion dur=""6s"" repeatCount=""indefinite"" path=""M0,0 L0,2070""/>
      </use>
      <use href=""#flag-es"" width=""50"" height=""33"" x=""600""  y=""-120"">
        <animateMotion dur=""4.5s"" repeatCount=""indefinite"" path=""M0,0 L0,2040""/>
      </use>
      <use href=""#flag-es"" width=""70"" height=""47"" x=""900""  y=""-130"">
        <animateMotion dur=""5.5s"" repeatCount=""indefinite"" path=""M0,0 L0,2050""/>
      </use>
      <use href=""#flag-es"" width=""65"" height=""43"" x=""450""  y=""-140"">
        <animateMotion dur=""4.8s"" repeatCount=""indefinite"" path=""M0,0 L0,2060""/>
      </use>

      <!-- Desde abajo -->
      <use href=""#flag-es"" width=""55"" height=""37"" x=""100""  y=""2050"">
        <animateMotion dur=""6s"" repeatCount=""indefinite"" path=""M0,0 L0,-2150""/>
      </use>
      <use href=""#flag-es"" width=""85"" height=""57"" x=""350""  y=""2020"">
        <animateMotion dur=""5s"" repeatCount=""indefinite"" path=""M0,0 L0,-2190""/>
      </use>
      <use href=""#flag-es"" width=""45"" height=""30"" x=""700""  y=""2040"">
        <animateMotion dur=""4s"" repeatCount=""indefinite"" path=""M0,0 L0,-2180""/>
      </use>
      <use href=""#flag-es"" width=""90"" height=""60"" x=""950""  y=""2060"">
        <animateMotion dur=""6.5s"" repeatCount=""indefinite"" path=""M0,0 L0,-2200""/>
      </use>
      <use href=""#flag-es"" width=""75"" height=""50"" x=""550""  y=""2030"">
        <animateMotion dur=""5.2s"" repeatCount=""indefinite"" path=""M0,0 L0,-2160""/>
      </use>

      <!-- Desde la izquierda -->
      <use href=""#flag-es"" width=""60"" height=""40"" x=""-100"" y=""200"">
        <animateMotion dur=""5s"" repeatCount=""indefinite"" path=""M0,0 L1180,0""/>
      </use>
      <use href=""#flag-es"" width=""80"" height=""53"" x=""-120"" y=""600"">
        <animateMotion dur=""6.2s"" repeatCount=""indefinite"" path=""M0,0 L1200,0""/>
      </use>
      <use href=""#flag-es"" width=""50"" height=""33"" x=""-90""  y=""1000"">
        <animateMotion dur=""4.3s"" repeatCount=""indefinite"" path=""M0,0 L1170,0""/>
      </use>
      <use href=""#flag-es"" width=""70"" height=""47"" x=""-110"" y=""1400"">
        <animateMotion dur=""5.7s"" repeatCount=""indefinite"" path=""M0,0 L1190,0""/>
      </use>
      <use href=""#flag-es"" width=""65"" height=""43"" x=""-80""  y=""1600"">
        <animateMotion dur=""4.9s"" repeatCount=""indefinite"" path=""M0,0 L1185,0""/>
      </use>

      <!-- Desde la derecha -->
      <use href=""#flag-es"" width=""55"" height=""37"" x=""1120"" y=""300"">
        <animateMotion dur=""5.5s"" repeatCount=""indefinite"" path=""M0,0 L-1180,0""/>
      </use>
      <use href=""#flag-es"" width=""85"" height=""57"" x=""1090"" y=""700"">
        <animateMotion dur=""6.4s"" repeatCount=""indefinite"" path=""M0,0 L-1200,0""/>
      </use>
      <use href=""#flag-es"" width=""45"" height=""30"" x=""1150"" y=""1100"">
        <animateMotion dur=""4.1s"" repeatCount=""indefinite"" path=""M0,0 L-1170,0""/>
      </use>
      <use href=""#flag-es"" width=""90"" height=""60"" x=""1100"" y=""1500"">
        <animateMotion dur=""6.8s"" repeatCount=""indefinite"" path=""M0,0 L-1190,0""/>
      </use>
      <use href=""#flag-es"" width=""75"" height=""50"" x=""1080"" y=""1750"">
        <animateMotion dur=""5.3s"" repeatCount=""indefinite"" path=""M0,0 L-1185,0""/>
      </use>
    </g>

    <!-- Texto central -->
    <g transform=""translate(540,960)"">
      <text
        text-anchor=""middle""
        dominant-baseline=""middle""
        fill=""#fff""
        font-family=""'Bebas Neue', sans-serif""
        font-size=""150""
        font-weight=""bold""
        opacity=""0""
      >
        LECTURAINTELIGENTE
        <animate attributeName=""opacity"" from=""0"" to=""1""
          begin=""1.5s"" dur=""0.5s"" fill=""freeze""/>
        <animateTransform attributeName=""transform"" type=""scale""
          values=""0.8;1.1;1"" keyTimes=""0;0.5;1""
          begin=""1.5s"" dur=""0.5s"" fill=""freeze""/>
        <animateTransform attributeName=""transform"" type=""scale""
          values=""1;1.015;1"" keyTimes=""0;0.5;1""
          begin=""2.2s"" dur=""2s"" repeatCount=""indefinite""/>
      </text>
    </g>

    <!-- Crédito -->
    <text
      x=""50%"" y=""1850""
      text-anchor=""middle""
      fill=""#fff""
      font-family=""Poppins, Arial, sans-serif""
      font-size=""50""
      opacity=""0""
    >
      by Marco &amp; Pau
      <animate attributeName=""opacity"" from=""0"" to=""1""
        begin=""2s"" dur=""0.5s"" fill=""freeze""/>
    </text>
  </svg>
</body>
</html>
";

            SplashWebView.Source = new HtmlWebViewSource { Html = html };

            // Transición a AppShell tras 5 segundos
            Device.StartTimer(TimeSpan.FromSeconds(7), () =>
            {
                Application.Current.MainPage = App.Services.GetRequiredService<AppShell>();
                return false; // sólo una vez
            });
        }
    }
}

