using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Динамические
{
    public interface IHtmlElement
    {
        void ToHtml(StringBuilder html);

    }


    public class Html : IHtmlElement
    {
        StringBuilder html = new StringBuilder();
        public IHtmlElement Body;
        public void ToHtml(StringBuilder html1)
        {
          
            html.Append( "<html>");
            Body.ToHtml(html);
            html.Append("</html>");
        }
        public string toHTMLString()
        {
            ToHtml(html);
            return html.ToString();
        }

    }
    public class Hbody : IHtmlElement
    {
        public List<IHtmlElement> list;
        public void ToHtml(StringBuilder html)
        {
            html.Append("<body>");
            foreach (var htmlElement in list)
            {
                htmlElement.ToHtml(html);
            }
            html.Append("</body>");
        }
    }

    public class Hdiv : IHtmlElement
    {
        public List<IHtmlElement>  list;
        //атрибуты
        public string font;
        public string style;

        public void ToHtml(StringBuilder html)
        {
            html.Append("<div>");
            foreach (var htmlElement in list)
            {
                htmlElement.ToHtml(html);
            }
            html.Append("</div>");
        }
    }

    public class HText :IHtmlElement
    {
        public string Text { get; set; } // здесь текст можно заменить на вычислимое значение

        public void ToHtml(StringBuilder html)
        {
            html.Append(Text);
        }
    }
    public class HВычисление : IHtmlElement
    {
        public string Вычисление { get; set; } // здесь текст можно заменить на вычислимое значение

        public void ToHtml(StringBuilder html)
        {
            html.Append(Вычисление);
        }
    }

    public class HttpServer
    {
        public HttpListener Server= new HttpListener();
        public Html Index;

        public void Start()
        {
            Server.Start();
            while (true)
            {
                HttpListenerContext context = Server.GetContext();
                HttpListenerRequest request = context.Request;

                // Obtain a response object.
                HttpListenerResponse ответ = context.Response;

                // Construct a response.
                string responseString = Index.toHTMLString();

                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                ответ.ContentLength64 = buffer.Length;

                var output = ответ.OutputStream;
                output.Write(buffer, 0, buffer.Length);

                ответ.Close(); //отправить ответ на самом деле   
            }
            Server.Stop();

        }


    }


}
