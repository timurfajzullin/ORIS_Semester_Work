using MyHttpServer.Core.Templaytor;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using MyHttttpServer.Models;

namespace MyHtttpServer.Core.Templator
{
    public class CustomTemplator : ICustomTemplator
    {
        public string GetHtmlByTemplate(string template, string name)
        {
            return template.Replace("{name}", name);
        }

        public string GetHtmlByTemplate<T>(string template, T obj)
        {
            var replace = "";
            var properties = obj.GetType().GetProperties();
            foreach (var property in properties)
            {
                replace = template.Replace("{" + property.PropertyType + "}", property.GetValue(obj)?.ToString());
            }
            return replace;
        }

        public static string GetHtmlByTemplateWithURL(string url, string name)
        {
            var template = $@"<div class=""mocked-styled-394 p1tslz7y"">
                                <div class=""phws30f"" style=""opacity: 1;"">
                                    <div data-cy=""tile"" data-has-progress=""false"" class=""mocked-styled-33 t1lfo8oz"" style=""--t1lfo8oz-0: none; --t1lfo8oz-1: 0;"">
                                        <div class=""mocked-styled-27 irx2gae"">
                                            <a data-cy=""image-link"" href=""movie?name={name}"">
                                                <div class=""iebw1u"" data-cy=""image"" style=""background-image: url({url});"">
                                                </div>
                                            </a>
                                        </div>
                                        <div class=""mocked-styled-28 in56or7""></div>
                                    </div>
                                </div>
                             </div>";
            return template;
        }

        public string GetHtmlWithMoviePlayer(string html, string name)
        {
            var movie = @"<swiper-container init=""false"" class=""swiper-initialized swiper-horizontal swiper-backface-hidden"">
                            <swiper-slide role=""group"" aria-label=""1 / 1"" class=""swiper-slide-active"">
                                <div data-cy=""tab"" class=""mocked-styled-644 t2g7dy0"">
                                    <button data-disabled=""false"" color=""default"" class=""mocked-styled-1 b1oy04za""><span class=""mocked-styled-0 i1tw7fe7"" style=""--i1tw7fe7-0: 4.063vw; --i1tw7fe7-1: 13px; --i1tw7fe7-2: 0.677vw;""></span>Фильмы</button>
                                </div>

                                <div id=""video-container"" style=""width: 100%; justify-content: center"">
                                    <iframe id=""video-frame"" src=""https://vk.com/video_ext.php?oid=-190452322&id=456239102&hd=2""
                                            allow=""encrypted-media; fullscreen; picture-in-picture; screen-wake-lock;""
                                            frameborder=""0""
                                            allowfullscreen>
                                    </iframe>
                                </div></swiper-slide>
                        </swiper-container>";
            var past = @"<swiper-container init=""false"" class=""swiper-initialized swiper-horizontal swiper-backface-hidden"">
                        </swiper-container>";
            if (name == "Лед")
            {
                return html.Replace(past, movie);
            }

            return html;
        }

        public string ReplaceSpecificHtml(string template, string placeholder, string replacementHtml)
        {
            // Define the regex pattern to match {{placeholder}}
            string pattern = $"\\{{\\{{{Regex.Escape(placeholder)}\\}}\\}}";

            // Replace all matches of the pattern with the replacement HTML
            return Regex.Replace(template, pattern, replacementHtml);
        }

        public string GetUserHtml(string template)
        {
            string loginButtonHtml = @"<div class=""mocked-styled-758 lmygb9s"">
                <a href=""login"" class=""mocked-styled-759 t103wgns"">Войти</a>
            </div>";

            string loggedInHtml = @"<div data-cy=""avatar"" id=""avatar"" class=""mocked-styled-757 a17c5pon"">
            <div data-cy=""avatar"" class=""mocked-styled-95 akji77o"">
                <a href=""admin"">
                <svg width=""40"" height=""40"" viewBox=""0 0 40 40"" xmlns=""http://www.w3.org/2000/svg"">
                    <g fill=""none"" fill-rule=""evenodd"">
                        <circle fill=""#1F1F1F"" cx=""20"" cy=""20"" r=""20""></circle>
                        <g stroke=""#FFF"" stroke-width=""1.84"">
                            <path d=""M10.4 24.267v-8.534a5.333 5.333 0 0 1 5.333-5.333h8.534a5.333 5.333 0 0 1 5.333 5.333v8.534a5.333 5.333 0 0 1-5.333 5.333h-8.534a5.333 5.333 0 0 1-5.333-5.333Z""></path>
                            <path d=""M24.8 22.667S23.2 24.8 20 24.8c-3.2 0-4.8-2.133-4.8-2.133"" stroke-linecap=""round"" stroke-linejoin=""round""></path>
                            <path d=""M16.267 17.867a.533.533 0 1 1 0-1.067.533.533 0 0 1 0 1.067Zm7.466 0a.533.533 0 1 1 0-1.067.533.533 0 0 1 0 1.067Z"" fill=""#FFF"" fill-rule=""nonzero"" stroke-linecap=""round"" stroke-linejoin=""round""></path>
                        </g>
                    </g>
                </svg>
                </a>
            </div>
        </div>";
            
            
                string userHtml = @"<div data-cy=""paid-filter"" class=""mocked-styled-366 pgjba3y"">
               
                </div>";

                string loginedPanel =
                    @"                <div data-cy=""paid-filter"" class=""mocked-styled-366 pgjba3y"">
               <div data-cy=""switcher"" class=""mocked-styled-76 s4tshxk"">
                    <div data-cy=""pupy"" class=""mocked-styled-78 a1m27v4j""></div>
                      <button id=""pupybutton"" data-is-active=""false"" class=""mocked-styled-77 tk2hnas"" onclick=""setActiveButton(this)"">Все</button>
                      <button id=""pupybutton"" data-is-active=""false"" class=""mocked-styled-77 tk2hnas"" onclick=""setActiveButton(this)"">Бесплатно</button>
                      <button id=""pupybutton"" data-is-active=""false"" class=""mocked-styled-77 tk2hnas"" onclick=""setActiveButton(this)"">По подписке</button>
                   </div>
                </div>
                </div>";
            return template.Replace(loginButtonHtml, loggedInHtml).Replace(userHtml, loginedPanel);
        }
        
        public string GetMovieHtml(string template)
        {
            string loginButtonHtml = @"<div data-cy=""enter"" class=""mocked-styled-756 ebzdbck"">
                    <div class=""mocked-styled-758 lmygb9s"">
                        <a href=""login"" class=""mocked-styled-759 t103wgns"">Войти</a>
                    </div>
                </div>";

            // Новый HTML с аватаром
            string loggedInHtml = @"<div data-cy=""avatar"" id=""avatar"" class=""mocked-styled-757 a17c5pon"">
            <div data-cy=""avatar"" class=""mocked-styled-95 akji77o"">
                <a href=""admin"">
                <svg width=""40"" height=""40"" viewBox=""0 0 40 40"" xmlns=""http://www.w3.org/2000/svg"">
                    <g fill=""none"" fill-rule=""evenodd"">
                        <circle fill=""#1F1F1F"" cx=""20"" cy=""20"" r=""20""></circle>
                        <g stroke=""#FFF"" stroke-width=""1.84"">
                            <path d=""M10.4 24.267v-8.534a5.333 5.333 0 0 1 5.333-5.333h8.534a5.333 5.333 0 0 1 5.333 5.333v8.534a5.333 5.333 0 0 1-5.333 5.333h-8.534a5.333 5.333 0 0 1-5.333-5.333Z""></path>
                            <path d=""M24.8 22.667S23.2 24.8 20 24.8c-3.2 0-4.8-2.133-4.8-2.133"" stroke-linecap=""round"" stroke-linejoin=""round""></path>
                            <path d=""M16.267 17.867a.533.533 0 1 1 0-1.067.533.533 0 0 1 0 1.067Zm7.466 0a.533.533 0 1 1 0-1.067.533.533 0 0 1 0 1.067Z"" fill=""#FFF"" fill-rule=""nonzero"" stroke-linecap=""round"" stroke-linejoin=""round""></path>
                        </g>
                    </g>
                </svg>
                </a>
            </div>
        </div>";

            string logButtonComents = @"<div data-cy=""input-simple"" class=""mocked-styled-146 i1c8416b"">
                                    <textarea class=""cxoxcvs tm9xqp6"" disabled="""" placeholder=""Войди на сайт, чтобы оставить комментарий"" maxlength=""255"" rows=""1"" spellcheck=""true""></textarea>
                                </div>
                                <button data-disabled=""false"" color=""transparent"" data-cy=""sign-in-button"" class=""mocked-styled-3 b1oy04za tlavnz7 b1oy04za"">
                                    <a href=""login""><span class=""mocked-styled-0 i1tw7fe7"" style=""--i1tw7fe7-0: 4.063vw; --i1tw7fe7-1: 13px; --i1tw7fe7-2: 0.677vw;""></span>Войти</a>
                                </button>";

            string update = @"<div data-cy=""input-simple"" class=""mocked-styled-146 i1c8416b"">
                                    <textarea class=""cxoxcvs tm9xqp6"" placeholder=""Написать комментарий"" maxlength=""255"" rows=""1"" spellcheck=""true""></textarea>
                                </div>
<button data-disabled=""false"" color=""transparent"" data-cy=""send-comment-button"" class=""mocked-styled-3 b1oy04za tlavnz7 b1oy04za""><span class=""mocked-styled-0 i1tw7fe7"" style=""--i1tw7fe7-0: 4.063vw; --i1tw7fe7-1: 13px; --i1tw7fe7-2: 0.677vw;""></span>Отправить </button>";
                
            return template.Replace(logButtonComents, update).Replace(loginButtonHtml, loggedInHtml);
        }
        
        public string GetHtmlByTemplateFilmPageData(Movie movies, string template) 
        {
            template = template.Replace("{{PosterUrl}}", movies.PosterURl);
            template = template.Replace("{{Title}}", movies.Name);
            template = template.Replace("{{Description}}", movies.Description);
            template = template.Replace("{{Starring}}", movies.Starring);
            template = template.Replace("{{Production}}", movies.Production);
            return template;
        }

    }
}
