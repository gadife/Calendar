using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CalendarManager.Models
{
    public class CheckboxValues
    {
        private List<CheckBox> m_boxes;

        public CheckboxValues ()
	    {
            m_boxes= new List<CheckBox>();
            IsSelfView = false;
	    }   

        public void Add(string name , bool selected = true)
        {
            m_boxes.Add(new CheckBox(name, selected));
        }

        public CheckBox[] CheckBoxes { get { return m_boxes.ToArray(); } }
        public string UserEmail { get; set; }
        public bool IsSelfView { get; set; }
    }

    public class CheckBox
    {
        public CheckBox(string name , bool selected = true)
        {
            Name = name;
            Selected = selected;
        }

        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}