using System.Xml.Linq;

namespace TagCleaner
{
    /// <summary>
    /// Utility class to clean up XML by replacing PackageReference elements that only have a nested Version element.
    /// </summary>
    public class Cleaner
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="xmlSource"></param>
        /// <exception cref="ArgumentException"></exception>
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

        /// <summary>
        /// Read the XML, replace PackageReference elements that only have nested Version element.
        /// </summary>
        /// <returns></returns>
        public string Clean()
        {
            // Find packages with Version elements only and no other elements
            var toReplace = doc.Descendants("PackageReference")
                .Where(pr => pr.Elements("Version").Any()
                    && !pr.Elements().Any(e => e.Name != "Version"))
                .ToList();
          
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
        }
    }
}
