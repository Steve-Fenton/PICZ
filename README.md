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

## Introduction

### Replacement for img Tags

You can use Picz wherever you place an image tag.
    
The `img` tag below can be replaced by a call to the Picz HTML helper.

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

### Replacement for background-image

You can use Picz on background images too.

The element before Picz was:

    <div id="picz-001" style="background-image: url(/Content/landscape-mountains-nature-lake.jpeg); background-size: cover; width: 50vw; height: 50vw; margin: 0 auto;">
        &nbsp;
    </div>

Picz is introuduced using the below code:

    @Html.PiczBackground("/Content/landscape-mountains-nature-lake.jpeg", "picz-001")
    <div id="picz-001" style="background-size: cover; width: 50vw; height: 50vw; margin: 0 auto;">
        &nbsp;
    </div>

Explanation:
    
The PiczBackground call is placed before the element in question, allowing you to keep full control of the element.
    
The first argument is simply the URL that you had in the style attribute already:

    /Content/landscape-mountains-nature-lake.jpeg

The second argument is the HTML id attribute of the element to target. This must be unique within the document.

    "picz-001"

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

## How it Works

Using the Picz HTML helper generates a responsive image based on a single image you 
have on your website. The source URLs for all of the image in the responsive image 
set are modified to point at a special controller action that will generate and cache 
images of the correct size.

### HTML Generation

Original:

    <img src="/image/example.png" alt="Unresponsive Image">

Picz HTML Helper:

    @Html.Picz("/image/example.png", "100vw", new { alt = "Responsive Image" })

Generated HTML:

    <img alt="Responsive Image" 
         sizes="100vw" 
         src="/picz?s=320&amp;p=/image/example.png"
         srcset="/picz?s=2500&amp;p=/image/example.png 2500w, /picz?s=1024&amp;p=/image/example.png 1024w, /picz?s=640&amp;p=/image/example.png 640w, /picz?s=320&amp;p=/image/example.png 320w">

The `src` attribute is set to a small version of the image. This has two purposes:

 - Fallback for older browsers
 - Placeholder for fast perceived loading times

The `srcset` contains a list of images for different view sizes, each of the images passes 
through the PiczController for processing.

The PiczController takes each request and serves the file either by generating it for the first time, 
or by delivering it straight from the disk cache.

## Image Selection and Hinting

Browsers will select from the list of available sizes, making a selection based on the view 
size, and the hint of the image size.

So for example, if the browser was set to 900px wide, it would calculate:

    900px x 100vw = 900px

It would then select the most appropriate image, i.e. the 1024px image.

If your image will be less than full width, you can specify this in the sizes argument:

    @Html.Picz("/image/example.png", "20vw", new { alt = "Responsive Image" })

The example above hints to the browser that the image will be one fifth of the view width.

You can also specify more advanced rules. A common example is where the image will be one-third of the 
view width, except on small devices where it will be the whole width:

    @Html.Picz("/image/example.png", "(min-width: 36em) 33.3vw, 100vw", new { alt = "Responsive Image" })

## Overriding Default Route

Picz uses the following route by default:

    /picz?s=320&p=/image/example.png

You can change this route with the following configuration in your `appSettings`:

    <add key="PiczRoute" value="imgz" />

Make sure you have set the route in your MVC application to match, i.e.

    [Route("imgz")]

## Overriding Default Sizes

Picz uses the following sizes by default:

 - 4000
 - 2500
 - 1024
 - 640
 - 320

You can set your own sizes using the following configuration to your `appSettings`:

    <add key="PiczSizes" value="3000,1500,750,375" />

## Disk Cache Size

Depending on your usage, you may wish to calculate the potential size of the cache.

Here are some examples.

### Big Source File

A common problem is when a user uploads a large source file (much bigger than the size it will be displayed).

Source PNG

    34.4 MB

Size of cache files for ALL sizes (4000, 2500, 1024, 640, 320)

    2.3 MB

Size sent to typical desktop (35 times smaller)

    625.82 KB

Size sent to typical mobile handset (1,780 times smaller)

    19.33 KB

To calculate your cache size where your source files are much bigger than the files you will generate, you'll need to 
generate at least the largest size first - then use that size as a guide.

### Source File Equivalant to Largest Size

If your source file is roughly equivalent to your largest size, you can roughly calculate the cache size as follows.

    Source * 2 = ESTIMATED SIZE

i.e.

    1.4 MB * 2 = 2.8 MB

This example is based on the default sizes (4000, 2500, 1024, 640, 320). Essentially, the 2,500, 1,024, 640, and 320 images 
add up to approximately the same as the 4,000 size image.

Your mileage will vary, depending on your images and the number of sizes you generate.

So you could calculate the total cache size by doubling your total image size - then add some extra capacity as a 
buffer and make sure you monitor disk use.

For enterprise use, you may wish to place the cache files on a separate disk or on a SAN

You may also want to clean out old files (ones that were cached, but are no longer used). You can set this up as 
a scheduled task in Windows, using this [example that was created to delete old IIS log files](https://www.stevefenton.co.uk/2015/07/clean-out-old-iis-log-files/).