using Microsoft.AspNetCore.Mvc;

namespace ToolsWebsite.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        [Route("sitemap.xml")]
        public IActionResult SitemapXml()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            
            var sitemap = $@"<?xml version=""1.0"" encoding=""UTF-8""?>
<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">
    <url>
        <loc>{baseUrl}/</loc>
        <lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>
        <changefreq>weekly</changefreq>
        <priority>1.0</priority>
    </url>
    <url>
        <loc>{baseUrl}/fcm</loc>
        <lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>
        <changefreq>monthly</changefreq>
        <priority>0.8</priority>
    </url>
    <url>
        <loc>{baseUrl}/qr</loc>
        <lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>
        <changefreq>monthly</changefreq>
        <priority>0.8</priority>
    </url>
    <url>
        <loc>{baseUrl}/auth</loc>
        <lastmod>{DateTime.UtcNow:yyyy-MM-dd}</lastmod>
        <changefreq>monthly</changefreq>
        <priority>0.8</priority>
    </url>
</urlset>";

            return Content(sitemap, "application/xml");
        }

        [Route("robots.txt")]
        public IActionResult RobotsTxt()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            
            var robots = $@"User-agent: *
Allow: /

Sitemap: {baseUrl}/sitemap.xml

Allow: /fcm
Allow: /qr 
Allow: /auth
Allow: /css/
Allow: /js/
Allow: /lib/

Crawl-delay: 1";

            return Content(robots, "text/plain");
        }
    }
}