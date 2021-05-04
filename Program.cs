using System;
using System.Net;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace pilgrim
{
    class Program
    {
        static void Main(string[] args)
        {
            var url = new StringBuilder();
            String urlAux = "https://thepilgrim.com.br/catalogue/product/";
            //string tipo, idioma, editora, titulo, autor;
            string teste, content;
            Boolean ftipo, fidioma, feditora, fautor, fautor2;
            HtmlDocument doc;
            HtmlNode node;
            List<HtmlNode> nodes;
            string[] tipos = new string[6654];
            string[] idiomas = new string[6654];
            string[] editoras = new string[6654];
            string[] titulos = new string[6654];
            string[] autores = new string[6654];
            //Encoding iso = Encoding.GetEncoding("ISO-8859-1");
            //Encoding utf8 = Encoding.UTF8;
            //byte[] utfBytes;
            //byte[] isoBytes;

            for (int i = 1; i < 6654; i++)
            {
                ftipo = false;
                fidioma = false;
                feditora = false;
                fautor = false;
                fautor2 = false;

                url.Clear();
                url.Append(urlAux);
                url.Append(i);

                using var client = new WebClient();

                try
                {
                    content = client.DownloadString(url.ToString());

                    //utfBytes = utf8.GetBytes(content);
                    //isoBytes = Encoding.Convert(utf8, iso, utfBytes);
                    //content = iso.GetString(isoBytes);

                    doc = new HtmlDocument();
                    doc.LoadHtml(content);
                    node = doc.DocumentNode;

                    nodes = node.Descendants().Where
                        (x => (x.Name == "h3" && x.Attributes["class"] != null &&
                        x.Attributes["class"].Value.Contains("title"))).ToList();

                    if (nodes.Count > 0)
                    {
                        titulos[i] = nodes[0].InnerText;
                    }
                    else
                    {
                        nodes = node.Descendants().Where
                        (x => (x.Name == "title")).ToList();
                        if (nodes.Count > 0)
                        {
                            titulos[i] = nodes[0].InnerText.Replace(" - The Pilgrim", String.Empty);
                        }
                    }

                    nodes = node.Descendants().Where
                        (x => (x.Name == "p" && x.Attributes["class"] != null &&
                        x.Attributes["class"].Value.Contains("contributor"))).ToList();

                    if (nodes.Count > 0)
                    {
                        autores[i] = nodes[0].InnerText;
                    }
                    else
                    {
                        fautor = true;
                    }

                    nodes = node.Descendants().Where
                        (x => (x.Name == "span" && x.Attributes["class"] != null &&
                        x.Attributes["class"].Value.Contains("product-info"))).ToList();

                    for (int j = 0; j < nodes.Count; j++)
                    {
                        teste = nodes[j].InnerText;

                        if (ftipo)
                        {
                            tipos[i] = teste;
                            ftipo = false;
                            continue;
                        }
                        if (fidioma)
                        {
                            idiomas[i] = teste;
                            fidioma = false;
                            continue;
                        }
                        if (feditora)
                        {
                            editoras[i] = teste;
                            feditora = false;
                            fautor2 = true;
                            continue;
                        }
                        if (fautor && fautor2)
                        {
                            autores[i] = teste;
                            continue;
                        }

                        switch (teste)
                        {
                            case "TIPO:":
                                ftipo = true;
                                break;
                            case "IDIOMA:":
                                fidioma = true;
                                break;
                            case "EDITORA:":
                                feditora = true;
                                break;
                            default:
                                break;
                        }
                    }
                    Console.WriteLine(i);
                }
                catch (System.Net.WebException)
                {

                }
            }

            var csv = new StringBuilder();
            string newLine;
            var filePath = @"C:\Users\Andreas\Desktop\pilgrim\pilgrim.csv";
            for (int i = 1; i < 6654; i++)
            {
                newLine = string.Format("{0}§{1}§{2}§{3}§{4}§{5}", i, titulos[i] ?? String.Empty, autores[i] ?? String.Empty, tipos[i] ?? String.Empty, idiomas[i] ?? String.Empty, editoras[i] ?? String.Empty);
                csv.AppendLine(newLine);
                Console.WriteLine(i);
            }
            File.WriteAllText(filePath, csv.ToString());

            Console.ReadLine();
        }
    }
}
