using System;
using System.Collections.Generic;
using System.Text;

namespace wform_v3.Models
{
    public class Link
    {
        public int Number { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }
        public bool Checked { get; set; }
        public override string ToString()
        {
            return $"{Number}) {Url}";
        }
    }
}