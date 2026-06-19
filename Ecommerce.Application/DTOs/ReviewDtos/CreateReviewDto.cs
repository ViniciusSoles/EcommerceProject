using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.DTOs.ReviewDtos
{
    public class CreateReviewDto
    {
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
