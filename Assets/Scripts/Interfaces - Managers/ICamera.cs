using UnityEngine;

public interface ICamera {

	void Zoom(object sender);
	void Pan(object sender);
	void SetBoundries(float minX, float minY, float maxX, float maxY);
	void Move(Vector3 worldPos);
	//void SetMenuWidth(float width);
}
