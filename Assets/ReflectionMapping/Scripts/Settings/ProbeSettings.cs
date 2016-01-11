using UnityEngine;
using System;
using System.Xml.Serialization;

public struct  ProbeSettings
{
		[XmlElement]
		public float
				radius;
		[XmlElement]
		public float
				x;
		[XmlElement]
		public float
				y;
		[XmlElement]
		public float
				z;

		[XmlIgnore]
		public Vector3 Position { 
				get { return new Vector3 (this.x, this.y, this.z); }
				set { 
						this.x = value.x;
						this.y = value.y;
						this.z = value.z;
				}
		}

		[XmlIgnore]
		public Vector3 Radius { 
				get { return new Vector3 (this.radius, this.radius, this.radius); }
				set { this.radius = value.x; }
		}
}

