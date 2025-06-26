using System.Xml.Linq;

namespace TagCleaner.Tests
{
    [TestClass]
    public sealed class Cleaner_Tests
    {
        [TestMethod]
        public void Clean_ReturnsOriginalIfNoPackages()
        {
            const string xml = """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <TargetFramework>net6.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                </Project>
                """;
            const string expected = """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <TargetFramework>net6.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                </Project>
                """;

            var cleaner = new Cleaner(xml); 
            var actual = cleaner.Clean();
            
            AssertXmlEqual(expected, actual);   
            //Assert.AreEqual(expected, actual);
        }
        
        [TestMethod]
        public void Clean_ConvertsPackagesToInline()
        {
            const string xml = """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <TargetFramework>net6.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                  <ItemGroup>
                	  <PackageReference Include="PackageName">
                		  <Version>1.2.3</Version>
                	  </PackageReference>
                  </ItemGroup>
                </Project>
                """;
            const string expected = """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <TargetFramework>net6.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                  <ItemGroup>
                	  <PackageReference Include="PackageName" Version="1.2.3" />
                  </ItemGroup>
                </Project>
                """;

            var cleaner = new Cleaner(xml); 
            var actual = cleaner.Clean();   

            AssertXmlEqual(expected, actual);
            //Assert.AreEqual(expected, actual);
        }
        
        [TestMethod]
        public void Clean_RetainsInlinePackages()
        {
            const string xml = """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <TargetFramework>net6.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                  <ItemGroup>
                	  <PackageReference Include="PackageName">
                		  <Version>1.2.3</Version>
                	  </PackageReference>
                      <PackageReference Include="AnotherPackage" Version="4.5.6" />
                  </ItemGroup>
                </Project>
                """;
            const string expected = """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <TargetFramework>net6.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                  <ItemGroup>
                	  <PackageReference Include="PackageName" Version="1.2.3" />
                      <PackageReference Include="AnotherPackage" Version="4.5.6" />
                  </ItemGroup>
                </Project>
                """;

            var cleaner = new Cleaner(xml); 
            var actual = cleaner.Clean();   

            AssertXmlEqual(expected, actual);
        }
        
        [TestMethod]
        public void Clean_RetainsPackagesWithNonVersionElements()
        {
            const string xml = """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <TargetFramework>net6.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                  <ItemGroup>
                	  <PackageReference Include="PackageName">
                		  <Version>1.2.3</Version>
                	  </PackageReference>
                      <PackageReference Include="AnotherPackage" Version="4.5.6" />
                      <PackageReference Include="Microsoft.TypeScript.MSBuild">
                        <Version>4.7.4</Version>
                        <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
                        <PrivateAssets>all</PrivateAssets>
                      </PackageReference>
                  </ItemGroup>
                </Project>
                """;
            const string expected = """
                <Project Sdk="Microsoft.NET.Sdk">
                  <PropertyGroup>
                    <OutputType>Exe</OutputType>
                    <TargetFramework>net6.0</TargetFramework>
                    <ImplicitUsings>enable</ImplicitUsings>
                    <Nullable>enable</Nullable>
                  </PropertyGroup>
                  <ItemGroup>
                	  <PackageReference Include="PackageName" Version="1.2.3" />
                      <PackageReference Include="AnotherPackage" Version="4.5.6" />
                      <PackageReference Include="Microsoft.TypeScript.MSBuild">
                        <Version>4.7.4</Version>
                        <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
                        <PrivateAssets>all</PrivateAssets>
                      </PackageReference>
                  </ItemGroup>
                </Project>
                """;

            var cleaner = new Cleaner(xml); 
            var actual = cleaner.Clean();   

            AssertXmlEqual(expected, actual);
        }

        /// <summary>
        /// We expect an exception if the XML does not contain a <Project> element.
        /// </summary>
        [TestMethod]
        public void Clean_Throws_If_No_Project()
        {
            const string xml = """
                <Root>
                  <Test>
                  </Test>
                </Root>
                """;
            var cleaner = new Cleaner(xml); 

            Assert.Throws<InvalidOperationException>(() => cleaner.Clean());
        }

        /// <summary>
        /// Compares two XML strings for equality, ignoring formatting differences.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        private static void AssertXmlEqual(string expected, string actual)
        {
            var expectedDoc = XDocument.Parse(expected);
            var actualDoc = XDocument.Parse(actual);
            Assert.IsTrue(XNode.DeepEquals(expectedDoc, actualDoc), 
                $"XML documents are not equal.\nExpected:\n{expected}\nActual:\n{actual}");
        }
    }
}
