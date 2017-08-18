using AutoMapper;
using HavaBusinessObjects.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMVC.App_Start
{
    public class AutoMapperConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<AutoMapperMap>();
            });
        }
    }
}