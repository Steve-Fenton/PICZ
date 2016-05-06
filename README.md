# Picz

Image tools for ASP.NET MVC Applications

## Why Picz?

There are several benefits to Picz:

 - Low resolution image placeholders for speed
 - Higher quality images loaded asynchronously for larger views
 - Disk-caching of replacement images
 - Improved the perceived load time of your pages
 - Reduce the weight of your pages for smaller devices

The goal of Picz is to...

 > Enable resposive images in return for the smallest possible investment

Because responsive images can be a drag to set up and maintain, adoption rates are worse than they could be. Picz aims 
to give developers the opportunity to get all the benefits of responsive images, without the complexity and maintenence 
issues.

 - The HTML helper needs to be as simple as the image tag it replaces.

 - You shouldn't have to pre-generate loads of images, or keep a set of images in sync to use responsive images.

 - Your website should appear to be faster, not slower, to the end user.

## Acknowledgements

new-york.jpg used in the examples, by [Terabass](https://en.wikipedia.org/wiki/Times_Square#/media/File:New_york_times_square-terabass.jpg)
london.jpg used in the examples, by [Daniel Chapma](https://en.wikipedia.org/wiki/List_of_tallest_buildings_and_structures_in_London#/media/File:London_from_a_hot_air_balloon.jpg)
paris.jpg used in the examples, by [Simon Reinhardt](https://commons.wikimedia.org/wiki/File:Paris_at_night,_4_July_2013.jpg)

## Quick Start

Grab the engine from NuGet:

    PM> Install-Package Fenton.Picz

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
