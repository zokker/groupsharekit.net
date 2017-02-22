﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Sdl.Community.GroupShareKit.Exceptions;
using Sdl.Community.GroupShareKit.Helpers;
using Sdl.Community.GroupShareKit.Http;
using Sdl.Community.GroupShareKit.Models.Response;

namespace Sdl.Community.GroupShareKit.Clients
{
    public class PermissionClient: ApiClient,IPermissionClient
    {
        public PermissionClient(IApiConnection apiConnection) : base(apiConnection)
        {
        }

        /// <summary>
        /// Gets all <see cref="Permission"/>s.
        /// </summary>
        /// <remarks>
        /// This method requires authentication.
        /// See the <a href="http://sdldevelopmentpartners.sdlproducts.com/documentation/api">API documentation</a> for more information.
        /// </remarks>
        /// <exception cref="AuthorizationException">
        /// Thrown when the current user does not have permission to make the request.
        /// </exception>
        /// <exception cref="ApiException">Thrown when a general API error occurs.</exception>
        /// <returns>A list of <see cref="Permission"/>s.</returns>
        public Task<IReadOnlyList<Permission>> GetAll()
        {
            return ApiConnection.GetAll<Permission>(ApiUrls.Permission());
        }

        public Task<IReadOnlyList<PermissionsName>> GetPermissionsName()
        {
            return ApiConnection.GetAll<PermissionsName>(ApiUrls.PermissionName());
        }
    }
}
