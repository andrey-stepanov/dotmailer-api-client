﻿using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dotMailer.Api.WadlParser.Methods.Abstract
{
    public abstract class Method : CodeBuilder
    {
        private static readonly string[] primitiveTypes = { "string", "int", "bool", "guid", "datetime" };

        public string Path
        { get; set; }

        public string Name
        { get; set; }

        public string Id
        { get; set; }

        public string Description
        { get; set; }

        public readonly IList<Response> Responses = new List<Response>();

        public readonly IList<Parameter> Parameters = new List<Parameter>();

        protected abstract void AppendMethodRequest();

        public override string ToString()
        {
            AppendSummary();
            AppendMethod();
            return base.ToString();
        }

        private void AppendSummary()
        {
            if (string.IsNullOrEmpty(Description))
                return;

            AddLine(2, "/// <summary>");
            AddLine(2, "/// {0}", Description);
            AddLine(2, "/// </summary>");
        }

        private void AppendMethod()
        {
            var serviceResult = string.IsNullOrEmpty(ReturnType) ? "ServiceResult" : string.Format("ServiceResult<{0}>", ReturnType);
            var parameters = RenderParameters();
            AddLine(2, "public {0} {1}({2})", serviceResult, Name, parameters);
            AddLine(2, "{");

            AppendMethodBody();

            AddLine(2, "}");
        }

        private string RenderParameters()
        {
            var psb = new StringBuilder();
            foreach (var parameter in Parameters.OrderByDescending(x => x.Required))
                psb.Append(parameter);

            var parameters = psb.ToString();
            if (!string.IsNullOrEmpty(parameters))
                parameters = parameters.Substring(0, parameters.Length - 2);

            return parameters;
        }

        private string returnType;
        protected string ReturnType
        {
            get
            {
                if (returnType == null)
                {
                    var response = Responses.SingleOrDefault(x => x.ReturnType != null);
                    returnType = response == null ? string.Empty : response.ReturnType;
                }
                return returnType;
            }
        }

        private IList<Parameter> PrimitiveParameters
        {
            get { return Parameters.Where(x => primitiveTypes.Contains(x.DataType.ToLower())).ToList(); }
        }

        protected IList<Parameter> ComplexParameters
        {
            get { return Parameters.Where(x => !PrimitiveParameters.Contains(x)).ToList(); }
        }

        private void AppendMethodBody()
        {
            if (PrimitiveParameters.Any())
            {
                AddLine(3, "var request = new Request(\"{0}\", ", Path);
                AddLine(3, "new Dictionary<string, object>");
                AddLine(3, "{");

                foreach (var parameter in PrimitiveParameters)
                    AddLine(4, "{{ \"{0}\", {0} }}{1}", parameter.Name, parameter == Parameters.Last() ? "" : ",");

                AddLine(3, "});");
            }
            else
            {
                AddLine(3, "var request = new Request(\"{0}\");", Path);
            }

            AppendMethodRequest();
        }
    }
}