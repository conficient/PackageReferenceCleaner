using System.Xml.Linq;

namespace TagCleaner
{
    public class Cleaner
    {
        public Cleaner(string xmlSource)
        {
            try
            {
                doc = XDocument.Parse(xmlSource);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid XML source provided.", nameof(xmlSource), ex);
            }
        }

        private readonly XDocument doc;

        public string Clean()
        {
            var project = doc.Element("Project");
            if (project == null) 
                throw new InvalidOperationException("The XML does not contain a <Project> element.");

            // Find packages with Version elements only and no other elements
            var packages = project.Descendants("PackageReference")
                .Where(pr => pr.Elements("Version").Any()
                    && !pr.Elements().Any(e => e.Name != "Version"))
                .ToList();
            //var toReplace = project
            //    .Descendants("PackageReference")
            //    .Where(pr =>
            //        pr.HasAttributes &&
            //        pr.Attribute("Include") != null &&
            //        pr.Attribute("Version") != null &&
            //        !pr.Elements().Any()
            //    )
            //    .ToList();

            // only pacakges with no other elements
            var toReplace = packages;
                //.Where(pr => !pr.Elements().Any(e => e.Name != "Version"))
                //.ToList();


            foreach (var pr in toReplace)
            {
                // Create new inline element
                var include = pr.Attribute("Include")?.Value;
                var version = pr.Element("Version")?.Value; 

                if (!string.IsNullOrEmpty(include) && !string.IsNullOrEmpty(version))
                {
                    var newElement = new XElement("PackageReference",
                        new XAttribute("Include", include),
                        new XAttribute("Version", version)
                    );
                    pr.ReplaceWith(newElement);
                }
            }

            var result = doc.ToString(SaveOptions.None);
            return result;
            //doc.Save(csprojPath);
        }
    }
}
