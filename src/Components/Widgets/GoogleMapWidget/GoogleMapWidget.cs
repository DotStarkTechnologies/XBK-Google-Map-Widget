using CMS.Core;
using CMS.Helpers;

using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc;

using DotStark.XBK.GoogleMapWidget;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using System;
using System.ComponentModel.DataAnnotations;

[assembly: RegisterWidget(
    identifier: GoogleMapWidgetViewComponent.IDENTIFIER, typeof(GoogleMapWidgetViewComponent),
    name: "Google Map",
    propertiesType: typeof(GoogleMapWidgetProperties),
    Description = "Show google map by latitude and longitude value using google API key.",
    IconClass = "icon-map-marker",
    AllowCache = true)]

namespace DotStark.XBK.GoogleMapWidget;

/// <summary>
/// Class which constructs the <see cref="GoogleMapWidgetViewModel"/> and renders the widget.
/// </summary>
public class GoogleMapWidgetViewComponent : ViewComponent
{
    /// <summary>
    /// The internal identifier of the Google Map widget.
    /// </summary>
    public const string IDENTIFIER = "DotStark.XBK.Widget.GoogleMap";

    private readonly IConfiguration configuration;
    private readonly IEventLogService eventLogService;

    /// <summary>
    /// Initializes a new instance of the <see cref="GoogleMapWidgetViewComponent"/> class.
    /// </summary>
    public GoogleMapWidgetViewComponent(IConfiguration configuration, IEventLogService eventLogService)
    {
        this.configuration = configuration;
        this.eventLogService = eventLogService;
    }

    /// <summary>
    /// Populates the <see cref="GoogleMapWidgetViewModel"/> and returns the appropriate view.
    /// </summary>
    /// <param name="widgetProperties">User populated properties from the page builder or view.</param>
    public IViewComponentResult Invoke(ComponentViewModel<GoogleMapWidgetProperties> widgetProperties)
    {
        try
        {
            if (widgetProperties == null || widgetProperties.Properties == null)
            {
                LogWidgetLoadError("Widget properties were not provided or are null.");
                return Content(String.Empty);
            }
            var properties = widgetProperties.Properties;
            var vm = new GoogleMapWidgetViewModel
            {
                IsVisible = properties.IsVisible,
                Latitude = ValidationHelper.GetString(properties.Latitude, ""),
                Longitude = ValidationHelper.GetString(properties.Longitude, ""),
                CssClass = properties.CssClassName,
                ApiKey = configuration.GetSection("GoogleMapsApiKey").Value //"GoogleMapsApiKey": "API Key Value",
            };
            return View("~/Components/Widgets/GoogleMapWidget/_GoogleMapWidget.cshtml", vm);
        }
        catch (Exception ex) 
        {
            LogWidgetLoadError(ex.Message);
            return Content(String.Empty);
        }
    }

    private void LogWidgetLoadError(string description)
    {
        eventLogService.LogError("Google Map Widget",
                "Load",
                description,
                new LoggingPolicy(TimeSpan.FromMinutes(1)));
    }
}

/// <summary>
/// The configurable properties for the GoogleMap widget.
/// </summary>
public class GoogleMapWidgetProperties: IWidgetProperties
{
    /// <summary>
    /// Indicates if widget is Visible on live site or not.
    /// </summary>
    [EditingComponent(CheckBoxComponent.IDENTIFIER, Order = 0, Label = "Visible", DefaultValue = true)]
    public bool IsVisible
    {
        get;
        set;
    }

    /// <summary>
    /// Latitude value is a coordinate for specific location and its mandatory 
    /// </summary>
    [EditingComponent(TextInputComponent.IDENTIFIER, Order = 1, Label = "Latitude *")]
    [Required(ErrorMessage = "The 'Latitude' is required.")]
    public string Latitude { get; set; }

    /// <summary>
    /// Longitude value is a coordinate for specific location and its mandatory
    /// </summary>
    [EditingComponent(TextInputComponent.IDENTIFIER, Order = 2, Label = "Longitude *")]
    [Required(ErrorMessage = "The 'Longitude' is required.")]
    public string Longitude 
    { 
        get; 
        set; 
    }

    /// <summary>
    /// The CSS class(es) added to the Google Map widget's containing DIV.
    /// </summary>
    [EditingComponent(TextInputComponent.IDENTIFIER, Order = 3, Label = "Css classes", ExplanationText = "Enter any number of CSS classes to apply to the Google Map Div, e.g. 'googlemap'")]
    public string CssClassName
    {
        get;
        set;
    } = "googlemap";
}

/// <summary>
/// The properties to be set when rendering the widget on a view.
/// </summary>
public class GoogleMapWidgetViewModel
{
    /// <summary>
    /// Indicates if widget is Visible on live site or not.
    /// </summary>
    public bool IsVisible
    {
        get;
        set;
    }

    /// <summary>
    /// Latitude value is a coordinate for specific location and its mandatory 
    /// </summary>
    public string Latitude;

    /// <summary>
    /// Longitude value is a coordinate for specific location and its mandatory
    /// </summary>
    public string Longitude;

    /// <summary>
    /// API Key for Google Map
    /// </summary>
    public string ApiKey 
    { 
        get; 
        set; 
    }

    /// <summary>
    /// CSS classes added to the containing DIV.
    /// </summary>
    public string CssClass
    {
        get;
        set;
    }
}






