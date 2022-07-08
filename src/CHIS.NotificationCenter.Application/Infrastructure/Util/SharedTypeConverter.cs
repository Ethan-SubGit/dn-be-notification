using System;
using System.Collections.Generic;
using System.Text;
using CHIS.Share.NotificationCenter.Models;
namespace CHIS.NotificationCenter.Application.Infrastructure.Util
{
    public class SharedTypeConverter
    {
        public static List<Models.CommonModels.ContentParameterDto> ConvertContentParameters(List<ContentParameterDto> contentParameterDtos)
        {
            List<Models.CommonModels.ContentParameterDto> list = new List<Models.CommonModels.ContentParameterDto>();
            if (contentParameterDtos ==  null)
            {
                return list;
            }

            foreach (var item in contentParameterDtos)
            {
                list.Add(new Models.CommonModels.ContentParameterDto() { ParameterValue = item.ParameterValue });
            }
            return list;
        }
    }
}
