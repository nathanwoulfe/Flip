using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flip.Api.Configuration;

internal class ApiConstants
{
    /// <summary>
    /// The API root path.
    /// </summary>
    public const string RootPath = "/umbraco/flip/management/api";

    /// <summary>
    /// The API group name.
    /// </summary>
    public const string ApiGroupName = "Flip";

    /// <summary>
    /// The API name.
    /// </summary>
    public const string ApiName = "flip-management";

    /// <summary>
    /// The API title.
    /// </summary>
    public const string ApiTitle = "Flip Management API";

    /// <summary>
    /// The namespace prefix for Flip API and related models.
    /// </summary>
    public const string ApiNamespacePrefix = "Flip.Api";
}
