using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valkyrie_VR
{
	public class PointCloudManager : MonoBehaviour
	{
		public ROSCore rosmaster;
		public GameObject pointCloudPrefab;
		public enum ColoringMethods
		{
			Flat,
			XAxisColored,
			YAxisColored,
			ZAxisColored,
			DistanceFromRobot
		}
		private ColoringMethods _coloringMethod = ColoringMethods.YAxisColored;
		public ColoringMethods coloringMethod = ColoringMethods.YAxisColored;

		private mesh_pcloud_rendering.PointShapes _pointShape = mesh_pcloud_rendering.PointShapes.TRIANGLE;
		public mesh_pcloud_rendering.PointShapes pointShape = mesh_pcloud_rendering.PointShapes.TRIANGLE;

		private float _tmax = 2f, _tmin = 1f;
		public float tmax = 2f, tmin = 1f;

		private mesh_pcloud_rendering[] clouds;
		// Use this for initialization
		void Start()
		{

		}

		void Update()
		{
			if (clouds == null)
			{
				return;
			}

			if (_coloringMethod != coloringMethod)
			{
				_coloringMethod = coloringMethod;
				foreach (mesh_pcloud_rendering cloud in clouds)
				{
					if (cloud == null) { continue; }
					else
					{
						cloud.coloringMethod = coloringMethod;
					}
				}
			}

			if (_tmax != tmax)
			{
				_tmax = tmax;
				foreach (mesh_pcloud_rendering cloud in clouds)
				{
					if (cloud == null) { continue; }
					else
					{
						cloud.ymax = tmax;
					}
				}
			}

			if (_tmin != tmin)
			{
				_tmin = tmin;
				foreach (mesh_pcloud_rendering cloud in clouds)
				{
					if (cloud == null) { continue; }
					cloud.ymin = tmin;
				}
			}

			if (_pointShape != pointShape)
			{
				_pointShape = pointShape;
				foreach (mesh_pcloud_rendering cloud in clouds)
				{
					if (cloud == null) { continue; }
					cloud.pointShape = pointShape;
				}
			}
		}
	}
}
