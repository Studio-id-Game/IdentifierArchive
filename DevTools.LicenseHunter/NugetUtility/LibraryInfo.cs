
/*
    this code from https://github.com/tomchavakis/nuget-license/blob/master/src/Model/LibraryInfo.cs

    #####################################################    

    Copyright 2020 Tom Chavakis, Lexy2, senslen

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.

    #####################################################    

    Edited by Studio Id Games

 */
namespace NugetUtility
{

    public class LibraryInfo
    {
        public string PackageName { get; set; } = "";
        public string PackageVersion { get; set; } = "";
        public string PackageUrl { get; set; } = "";
        public string Copyright { get; set; } = "";
        public string[] Authors { get; set; } = [];
        public string Description { get; set; } = "";
        public string LicenseUrl { get; set; } = "";
        public string LicenseType { get; set; } = "";
        public LibraryRepositoryInfo Repository { get; set; } = new LibraryRepositoryInfo();
    }

    public class LibraryRepositoryInfo
    {
        public string Type { get; set; } = "";
        public string Url { get; set; } = "";
        public string Commit { get; set; } = "";
    }
}
