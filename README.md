# PICZ

Image tools for ASP.NET MVC Applications

## Why PICZ?

There are several benefits to PICZ:

 - Low resolution image placeholders for speed
 - Higher quality images loaded asynchronously for larger views
 - Disk-caching of replacement images
 - Improved the perceived load time of your pages
 - Reduce the weight of your pages for smaller devices

## Introduction

You can use Picz wherever you place an image tag.
    
The image tag below can be replaced by Picz.

    <img src="~/Content/landscape-mountains-nature-lake.jpeg" alt="Mountain Reflection" /> 

The replacement code is:

    @Html.Picz(
         "~/Content/landscape-mountains-nature-lake.jpeg",
         "100vw",
         new { alt = "Mountain Reflection" })

The first argument is simply the URL that you had in the src attribute already:

    "~/Content/landscape-mountains-nature-lake.jpeg"
    
The second argument indicates to browsers what portion of the view the image will take up,
this allows the browser to choose the best image (i.e. if you will take up 50vw, and the view is
600px, it may choose a 300px image size).

    "100vw"

The third argument allows you to set additional HTML attributes, it is recommended that you
set the alt tag as a minimum.

    new { alt = "Mountain Reflection" }

## Quick Start

Grab the engine from NuGet:

    PM> Install-Package Fenton.Picz.Engine

Add the following configuration (example shows 48 hour caching, in a directory on the E drive).

    <add key="PiczCacheDurationHours" value="48" />
    <add key="PiczCachePath" value="E:\Temp\ImageCache\" />

Add a three line controller action to handle the image requests:

    using Fenton.Picz.Engine;
    using System;
    using System.Web.Mvc;

    public class PiczController : Controller
    {
        private readonly ImageResizer _imageResizer = new ImageResizer();

        [Route("Picz")]
        public ActionResult Picz(int s, string p)
        {
            var originalUrl = new Uri(Request.Url, p).AbsoluteUri;
            var replacement = _imageResizer.GetReplacementImage(s, originalUrl);
            return File(replacement.Path, replacement.MimeType);
        }
    }