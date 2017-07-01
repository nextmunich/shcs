using UnityEngine;
using System.Collections.Generic;

public class CameraOrderModel
{
    public string Id { get; set; }

    public IDictionary<string, string> CameraOrderItems { get; set; }

    public string Client { get; set; }

    public override string ToString()
    {
        return string.Format("[CameraOrderModel: Id={0}, CameraOrderItems={1}, Client={2}]", Id, CameraOrderItems, Client);
    }
}