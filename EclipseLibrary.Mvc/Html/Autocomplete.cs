﻿using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace EclipseLibrary.Mvc.Html
{
    [Obsolete("Create your own class")]
    public class AutocompleteItem
    {
        /// <summary>
        /// Text displayed in the list
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// The id which is posted back (e.g. SKU Id)
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// Friendly short name of the selected value (such as UPC code). Defaults to value
        /// </summary>
        public string shortName { get; set; }
    }

    /// <summary>
    /// Need a javascript function to reliably clear autocomplete input, including validation errors
    /// </summary>
    /// <remarks>
    /// <para>
    /// Several <see cref="AutocompleteFor"/> helpers are available, each designed for different scenarios. The Overload 1 takes lambdas for the value field and for
    /// the short name field. This is used when you want to postback the value (invisibly) selected by the user, and the short name (visibly) selected by the user. This overload
    /// also allows you to supply initial values for the invisible id and the visible short name simply by setting the values of the properties referenced by the lambda expressions.
    /// Ideally suited for SKU autocomplete where the hidden SKU Id needs to be posted back.
    /// </para>
    /// <para>
    /// Overload 2 takes only a single lambda expression for the value. This is for scenarios where your value is visible to the user, such as the Style autocomplete.
    /// </para>
    /// <para>
    /// Client API: $('#myautomplete').autocompleteEx('clear');
    /// To clear all autocomplete fields:  <c>$('input.ui-autocomplete-input').autocompleteEx('clear');</c>
    /// </para>
    /// </remarks>
    [Obsolete]
    public static class AutocompleteExtensions
    {
        /// <summary>
        /// Overload 1: Hidden field for value and visible textbox for short name
        /// </summary>
        /// <typeparam name="TModel">Type of the model</typeparam>
        /// <typeparam name="TProperty1">Type of the property for which autocomplete is being created.</typeparam>
        /// <typeparam name="TProperty2"></typeparam>
        /// <param name="helper">HTML helper</param>
        /// <param name="exprValue">Lambda expression referencing the property which will receive the Id. Client side validation is not performed on this property.</param>
        /// <param name="listUrl">Url to invoke which will return the list of choices. It will be passed a single parameter called term.</param>
        /// <param name="validateUrl">The Url to invoke to validate the user entry when the user does not select from a list. Optional but highly recommended.
        /// If not specified, then program will misbehave if user does not select from list.</param>
        /// <param name="htmlAttributes">HTML attributes to apply to the rendered text box</param>
        /// <param name="exprShortName"></param>
        /// <returns>Markup to render</returns>
        /// <remarks>
        /// <para>
        /// The list is designed to be non obtrusive. It appears only after a significant pause (2 sec). The user is free to type a value without choosing from a list.
        /// In this scenario, the action specified in <paramref name="validateUrl"/> will be invoked during the validation process 
        /// to obtain the value which the user would have selected from the list.
        /// </para>
        /// <para>
        /// Once the user makes a selection, the <see cref="AutocompleteItem.shortName"/> is selected in the text box and the
        /// <see cref="AutocompleteItem.value"/> is selected in the hidden field.
        /// The companion <see cref="AutocompleteDescriptionFor"/> helper should be used if it is desirable to show the description of what the user selected.
        /// The span generated by this helper will receive <see cref="AutocompleteItem.label"/>
        /// </para>
        /// <para>
        /// Sample markup. Notice that if you have applied validators, the validation attributes are rendered as well. The hidden field is rendered immediately after
        /// the visible text box. This is how the script identifies the hidden field.
        /// </para>
        /// <code>
        /// <![CDATA[
        /// <input name="exprValue" type="hidden" />
        /// <input type="text" name="exprShortName"
        ///   data-ac-list-url="/REQ2/AutoComplete/SkuAutocomplete"
        ///   data-ac-validate-url="/REQ2/AutoComplete/ValidateSku" />
        /// &darr;*
        /// ]]>
        /// </code>
        /// <para>
        /// You can customize autocomplete options by writing your own script. Pass an id to the <see cref="AutocompleteFor"/> helper like this:
        /// </para>
        /// <code>
        /// <![CDATA[
        ///@Html.AutocompleteFor(m => m.Style, Url.Action(MVC_Receiving.Receiving.AutoComplete.StyleAutocomplete()), null, new
        ///{
        ///    size = 15,
        ///    style = "text-transform: uppercase;",
        ///    id = "tbStyleAutocomplete"
        ///})
        /// ]]>
        /// </code>
        /// <para>
        /// Then somewhere in your <c>$(document).ready()</c> function, set the options.
        /// </para>
        /// <code>
        /// <![CDATA[
        ///$(document).ready(function () {
        ///    $('#tbStyleAutocomplete').autocompleteEx('option', 'minLength', 1);
        ///});
        /// ]]>
        /// 
        /// </code>
        /// 
        /// 
        /// </remarks>
        /// <example>
        /// <para>
        /// Include the script Autocomplete.partial.js. Include the style sheet Autocomplete.partial.css. Include the image Images/ajax-loader.gif.
        /// All these files are available in this library and you will need to copy them into your project.
        /// </para>
        /// <para>
        /// In the view:
        /// </para>
        /// <code>
        /// <![CDATA[
        ///@Html.AutocompleteFor(m => m.NewSourceSkuId, Url.Action(MVC_REQ2.REQ2.AutoComplete.SkuAutocomplete()),
        ///        Url.Action(MVC_REQ2.REQ2.AutoComplete.ValidateSku()), new
        ///     {
        ///         style = "text-transform: uppercase",
        ///         size = 18,
        ///         maxlength = 20
        ///})                  
        ///@Html.AutocompleteMessageFor(m => m.NewSourceSkuId)
        /// ]]>
        /// </code>
        /// <para>
        /// The action method must have a parameter called <c>term</c> which will be set to the text entered by the user.
        /// It must return a list of <see cref="AutocompleteItem"/> as JSON.
        /// </para>
        /// <code>
        /// <![CDATA[
        ///public ActionResult SkuAutocomplete(string term)
        ///{
        ///    var data = _repos.UpcAutoComplete(term.ToUpper());
        ///    return Json(Mapper.Map<IEnumerable<AutoCompleteItem>>(data), JsonRequestBehavior.AllowGet);
        ///}
        /// ]]>
        /// </code>
        /// <para>
        /// The SKU model is mapped to <see cref="AutocompleteItem"/> like this:
        /// </para>
        /// <code>
        /// <![CDATA[
        ///Mapper.CreateMap<SkuModel, AutocompleteExtensions.AutoCompleteItem>()
        ///    .ForMember(dest => dest.label, opt => opt.MapFrom(src => string.Format("{0},{1},{2},{3}", src.Style, src.Color, src.Dimension, src.SkuSize)))
        ///    .ForMember(dest => dest.value, opt => opt.MapFrom(src => src.SkuId))
        ///    .ForMember(dest => dest.shortName, opt => opt.MapFrom(src => src.UpcCode))
        ///    ;
        /// ]]>
        /// </code>
        /// <para>
        /// Another action method is needed which is responsible for validating user input when the user does not select from a list. It needs to return a single
        /// <see cref="AutocompleteItem"/> or error message based on the text entered by the user.
        /// </para>
        /// <code>
        /// <![CDATA[
        ///[HandleAjaxError(true)]
        ///[HttpGet]
        ///public virtual JsonResult ValidateSku()
        ///{
        ///    if (Request.QueryString.Count == 0)
        ///    {
        ///        throw new ApplicationException("Nothing to validate. Should not happen");
        ///    }
        ///    var barCode = Request.QueryString[0].ToUpper();
        ///    var sku = _repos.GetSkuFromUpc(barCode);
        ///    if (sku == null)
        ///    {
        ///        return Json(string.Format("No such SKU: {0}", barCode.ToUpper()), JsonRequestBehavior.AllowGet);
        ///    }
        ///    return Json(Mapper.Map<AutoCompleteItem>(sku), JsonRequestBehavior.AllowGet);
        ///}
        /// ]]>
        /// </code>
        /// </example>
        [Obsolete]
        public static MvcHtmlString AutocompleteFor<TModel, TProperty1, TProperty2>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty1>> exprValue,
            Expression<Func<TModel, TProperty2>> exprShortName,
            string listUrl, string validateUrl = null, object htmlAttributes = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(listUrl));

            var input = AutocompleteHiddenImpl(ExpressionHelper.GetExpressionText(exprValue), EvaluateLambda(helper, exprValue), null);

            var sb = new StringBuilder();
            sb.Append(input.ToString(TagRenderMode.SelfClosing));

            // Visible Textbox
            input = AutocompleteVisibleImpl(helper, ExpressionHelper.GetExpressionText(exprShortName), EvaluateLambda(helper, exprShortName),
                listUrl, validateUrl, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            sb.Append(input.ToString(TagRenderMode.SelfClosing));


            AutocompleteAfterVisibleImpl(sb, ModelMetadata.FromLambdaExpression(exprShortName, helper.ViewData).IsRequired);

            return MvcHtmlString.Create(sb.ToString());
        }

        /// <summary>
        /// Overload 2: Visible textbox for value. No short name
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="helper"></param>
        /// <param name="exprValue"></param>
        /// <param name="listUrl"></param>
        /// <param name="validateUrl"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        [Obsolete]
        public static MvcHtmlString AutocompleteFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> exprValue,
            string listUrl, string validateUrl = null, object htmlAttributes = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(listUrl));

            // Visible Textbox
            var input = AutocompleteVisibleImpl(helper, ExpressionHelper.GetExpressionText(exprValue), EvaluateLambda(helper, exprValue),
                listUrl, validateUrl, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            var sb = new StringBuilder();
            sb.Append(input.ToString(TagRenderMode.SelfClosing));
            AutocompleteAfterVisibleImpl(sb, ModelMetadata.FromLambdaExpression(exprValue, helper.ViewData).IsRequired);

            return MvcHtmlString.Create(sb.ToString());
        }

        /// <summary>
        /// Displays label of the selected value in the associated autocomplete
        /// </summary>
        /// <typeparam name="TModel">Type of the model. Automatically deduced</typeparam>
        /// <typeparam name="TProperty">Type of value displayed in the visible text box. Usually string.</typeparam>
        /// <param name="helper">HTML helper</param>
        /// <param name="expression">Lambda expression referncing the visible text box</param>
        /// <param name="htmlAttributes">HTML attributes to apply to the span element</param>
        /// <param name="html">Initial html to display within the span element</param>
        /// <returns>Markup for the span element to render</returns>
        /// <remarks>
        /// 
        /// </remarks>
        [Obsolete]
        public static MvcHtmlString AutocompleteDescriptionFor<TModel, TProperty>(this HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> expression,
            object htmlAttributes = null, string html = null)
        {

            return helper.AutocompleteDescription(ExpressionHelper.GetExpressionText(expression), htmlAttributes, html);
        }

        #region Advanced
        /// <summary>
        /// Advanced: For use in edit templates where field names are read from template info
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="nameValue"></param>
        /// <param name="nameShortName"></param>
        /// <param name="listUrl"></param>
        /// <param name="validateUrl"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        [Obsolete]
        public static MvcHtmlString Autocomplete(this HtmlHelper helper, string nameValue, string nameShortName,
            string listUrl, string validateUrl = null, object htmlAttributes = null)
        {
            Debug.Assert(!string.IsNullOrEmpty(listUrl));

            var input = AutocompleteHiddenImpl(nameValue, null, null);

            var sb = new StringBuilder();
            sb.Append(input.ToString(TagRenderMode.SelfClosing));

            // Visible Textbox
            input = AutocompleteVisibleImpl(helper, nameShortName, null, listUrl, validateUrl, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            sb.Append(input.ToString(TagRenderMode.SelfClosing));


            AutocompleteAfterVisibleImpl(sb, ModelMetadata.FromStringExpression(nameShortName, helper.ViewData).IsRequired);

            return MvcHtmlString.Create(sb.ToString());
        }

        [Obsolete]
        public static MvcHtmlString AutocompleteDescription(this HtmlHelper helper, string name,
            object htmlAttributes = null, string html = null)
        {
            var span = new TagBuilder("span");
            span.Attributes.Add("data-ac-msg-for", name);

            if (htmlAttributes != null)
            {
                span.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes), true);
            }

            if (html != null)
            {
                span.InnerHtml = html;
            }

            return MvcHtmlString.Create(span.ToString(TagRenderMode.Normal) + helper.ValidationMessage(name));
        }
        #endregion

        #region Implementation
        private static TagBuilder AutocompleteHiddenImpl(string name, string value, RouteValueDictionary htmlAttributes)
        {
            var input = new TagBuilder("input");
            input.Attributes.Add("type", "hidden");
            input.Attributes.Add("name", name);

            if (!string.IsNullOrEmpty(value))
            {
                input.Attributes.Add("value", value);
            }

            if (htmlAttributes != null)
            {
                input.MergeAttributes(htmlAttributes, true);
            }
            return input;
        }


        private static TagBuilder AutocompleteVisibleImpl(HtmlHelper helper, string name,
            string value, string listUrl, string validateUrl, RouteValueDictionary htmlAttributes)
        {
            // Visible Textbox
            var input = new TagBuilder("input");
            input.Attributes.Add("type", "text");
            input.Attributes.Add("data-ac-list-url", listUrl);
            if (!string.IsNullOrEmpty(validateUrl))
            {
                input.Attributes.Add("data-ac-validate-url", validateUrl);
            }
            input.Attributes.Add("name", name);
            if (!string.IsNullOrEmpty(value))
            {
                input.Attributes.Add("value", value);
            }

            input.MergeAttributes(helper.GetUnobtrusiveValidationAttributes(name), true);

            if (htmlAttributes != null)
            {
                input.MergeAttributes(htmlAttributes, true);
            }
            return input;
        }

        private static void AutocompleteAfterVisibleImpl(StringBuilder sb, bool isRequired)
        {
            sb.Append("<span title=\"Double click to quickly see suggestions\">&darr;</span>");

            if (isRequired)
            {
                sb.Append("<span title=\"Value is required\">*</span>");
            }
        }

        private static string EvaluateLambda<TModel, TProperty>(HtmlHelper<TModel> helper, Expression<Func<TModel, TProperty>> exprValue)
        {
            string value;
            try
            {
                var val = exprValue.Compile()(helper.ViewData.Model);
                var metadata = ModelMetadata.FromLambdaExpression(exprValue, helper.ViewData);
                value = string.Format(metadata.EditFormatString ?? "{0}", val);
            }
            catch (NullReferenceException)
            {
                value = string.Empty;
            }

            return value;
        }
        #endregion
    }
}
