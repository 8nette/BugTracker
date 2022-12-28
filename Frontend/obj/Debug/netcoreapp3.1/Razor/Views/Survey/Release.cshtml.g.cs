#pragma checksum "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "f3ba279ba70b025ac07f76f154e3ce0757eb1478"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Survey_Release), @"mvc.1.0.view", @"/Views/Survey/Release.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "/Users/anette/Projects/BugTracker2/Frontend/Views/_ViewImports.cshtml"
using Frontend;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "/Users/anette/Projects/BugTracker2/Frontend/Views/_ViewImports.cshtml"
using Frontend.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"f3ba279ba70b025ac07f76f154e3ce0757eb1478", @"/Views/Survey/Release.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"aa18a685f75738db7c974e4b1ed0d820d1291efd", @"/Views/_ViewImports.cshtml")]
    public class Views_Survey_Release : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<ReleaseModel>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\n");
#nullable restore
#line 3 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
  
    ViewData["Title"] = "Bug Tracker";

#line default
#line hidden
#nullable disable
            WriteLiteral("\n<div class=\"release\">\n    <h1>Release Planning for Release ");
#nullable restore
#line 8 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                                Write(Model.release);

#line default
#line hidden
#nullable disable
            WriteLiteral(" under Product ");
#nullable restore
#line 8 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                                                             Write(Model.product.Name);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h1>\n\n    <div class=\"releaseplan\">\n        <table>\n            <tr>\n                <td>From</td>\n                <td>");
#nullable restore
#line 14 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
               Write(Model.releasePlan.DateStart.ToString("dd/MM/yyyy"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n            </tr>\n            <tr>\n                <td>To</td>\n                <td>");
#nullable restore
#line 18 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
               Write(Model.releasePlan.DateEnd.ToString("dd/MM/yyyy"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n            </tr>\n            <tr>\n                <td>Workload</td>\n                <td>");
#nullable restore
#line 22 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
               Write(Model.releasePlan.Workload);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n            </tr>\n            <tr>\n                <td>Objectives</td>\n                <td>");
#nullable restore
#line 26 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
               Write(Model.releasePlan.Objectives);

#line default
#line hidden
#nullable disable
            WriteLiteral(@"</td>
            </tr>
        </table>
    </div>
    
    <div class=""features"">
        <h3>Release Features</h3>
        <table>
            <tr>
                <th>Title</th>
                <th>Area</th>
                <th>Developers</th>
                <th>Priority</th>
                <th>Resolution</th>
            </tr>

");
#nullable restore
#line 42 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
             foreach (Backend.Models.Bug feature in Model.features)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\n                    <td>");
#nullable restore
#line 45 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                   Write(feature.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>");
#nullable restore
#line 46 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                   Write(feature.Area);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>");
#nullable restore
#line 47 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                   Write(feature.Developers);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>");
#nullable restore
#line 48 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                   Write(feature.Priority);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>\n                        <p class=\"res\">");
#nullable restore
#line 50 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                                  Write(feature.Resolution);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\n                    </td>\n                </tr>\n");
#nullable restore
#line 53 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("\n");
#nullable restore
#line 55 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
             if (Model.bugs.Count() == 0)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\n                    <td></td>\n                    <td></td>\n                    <td></td>\n                    <td></td>\n                    <td></td>\n                </tr>\n");
#nullable restore
#line 64 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("        </table>\n    </div>\n\n    <div class=\"buglist\">\n        <h3>Release Bugs</h3>\n        <table>\n            <tr>\n                <th>Title</th>\n                <th>Priority</th>\n                <th>Resolution</th>\n            </tr>\n\n");
#nullable restore
#line 77 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
             foreach (Backend.Models.Bug bug in Model.bugs)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\n                    <td>");
#nullable restore
#line 80 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                   Write(bug.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>");
#nullable restore
#line 81 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                   Write(bug.Priority);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>\n                        <p class=\"res\">");
#nullable restore
#line 83 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                                  Write(bug.Resolution);

#line default
#line hidden
#nullable disable
            WriteLiteral("</p>\n                    </td>\n                </tr>\n");
#nullable restore
#line 86 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("\n");
#nullable restore
#line 88 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
             if (Model.bugs.Count() == 0)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\n                    <td></td>\n                    <td></td>\n                    <td></td>\n                </tr>\n");
#nullable restore
#line 95 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"        </table>
    </div>

    <div class=""tasks"">
        <h3>Tasks with a Release Bug</h3>
        <table>
            <tr>
                <th>Task</th>
                <th>Start</th>
                <th>End</th>
                <th>Release Bugs</th>
            </tr>

");
#nullable restore
#line 109 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
             if (Model.tasks.Count() == 0)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\n                    <td></td>\n                    <td></td>\n                    <td></td>\n                    <td></td>\n                </tr>\n");
#nullable restore
#line 117 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("\n");
#nullable restore
#line 119 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
             foreach (Backend.Models.Task task in Model.tasks)
            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                <tr>\n                    <td>");
#nullable restore
#line 122 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                   Write(task.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>");
#nullable restore
#line 123 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                   Write(task.Start.ToString("dd/MM/yyyy"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>");
#nullable restore
#line 124 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                   Write(task.End.ToString("dd/MM/yyyy"));

#line default
#line hidden
#nullable disable
            WriteLiteral("</td>\n                    <td>\n                        <ul>\n");
#nullable restore
#line 127 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                             foreach (Backend.Models.Bug bug in task.TaskBugs)
                            {

#line default
#line hidden
#nullable disable
            WriteLiteral("                                <li class=\"bug\">");
#nullable restore
#line 129 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                                           Write(bug.Title);

#line default
#line hidden
#nullable disable
            WriteLiteral("</li>\n");
#nullable restore
#line 130 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
                            }

#line default
#line hidden
#nullable disable
            WriteLiteral("                        </ul>\n                    </td>\n                </tr>\n");
#nullable restore
#line 134 "/Users/anette/Projects/BugTracker2/Frontend/Views/Survey/Release.cshtml"
            }

#line default
#line hidden
#nullable disable
            WriteLiteral("        </table>\n     </div>\n</div>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<ReleaseModel> Html { get; private set; }
    }
}
#pragma warning restore 1591
