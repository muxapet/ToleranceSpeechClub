using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private float _a, _b, _c, _t, _x, _y, _distance;
    private Vector3 _startPoint, _endPoint, _currentPoint;
    private Transform _transform;

    private bool _isThrowing = false;

    public void Throw(Transform from, Transform to)
    {
        _transform = transform;
        _startPoint = from.position;
        _endPoint = to.position;

        var p1 = from.position;
        var p2 = (to.position + from.position) / 2 + new Vector3(0, Random.Range(0.5f, 1f), 0);
        var p3 = to.position;

        var p3x = (p3 - p1).magnitude;
        var p2x = p3x / 2;
        _distance = p3x;

        _a = (p3.y - (p3x * (p2.y - p1.y) + p2x * p1.y) / p2x) / (p3x * (p3x - p2x));
        _b = (p2.y - p1.y) / p2x - _a * p2x;
        _c = 0;

        _x = 0;
        _t = 0;
        _isThrowing = true;
    }

    private void Update()
    {
        if (!_isThrowing) return;

        _t += Time.deltaTime;
        _x = _t * _distance;
        _y = _a * _x * _x + _b * _x;

        _currentPoint = Vector3.Lerp(_startPoint, _endPoint, _t);
        _currentPoint.y = _y + _startPoint.y;
        _transform.position = _currentPoint;

        if (_t >= 1)
        {
            _isThrowing = false;
        }
    }
}