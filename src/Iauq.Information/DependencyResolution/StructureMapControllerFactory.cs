﻿using System;
using System.Web.Mvc;
using System.Web.Routing;
using StructureMap;

namespace Iauq.Information.DependencyResolution
{
    public class StructureMapControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
                return base.GetControllerInstance(requestContext, null);

            return ObjectFactory.GetInstance(controllerType) as Controller;
        }
    }
}