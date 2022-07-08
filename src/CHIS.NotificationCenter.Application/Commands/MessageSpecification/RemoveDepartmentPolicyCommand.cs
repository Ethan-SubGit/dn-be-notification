﻿using System;
using System.Collections.Generic;
using System.Text;
using MediatR;


namespace CHIS.NotificationCenter.Application.Commands.MessageSpecification
{
    public class RemoveDepartmentPolicyCommand : IRequest<bool>
    {
        public string Id { get; set; }

        public string MessageSpecificationId { get; set; }

    }
}