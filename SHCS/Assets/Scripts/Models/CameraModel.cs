using UnityEngine;
using System.Collections;
using Newtonsoft.Json;

public class CameraModel
{
    public string Id { get; set; }

    public string Name { get; set; }

    [JsonProperty(PropertyName="cameraModel")]
    public CameraModelType CameraModelType { get; set; }

    public double PriceInEuro { get; set; }

    public double FieldOfViewInDeg { get; set; }

    public bool CanRotate { get; set; }

    public override string ToString()
    {
        return string.Format("[CameraModel: Id={0}, Name={1}, CameraModel={2}, PriceInEuro={3}, FieldOfViewInDeg={4}, CanRotate={5}]", Id, Name, CameraModelType, PriceInEuro, FieldOfViewInDeg, CanRotate);
    }
}
