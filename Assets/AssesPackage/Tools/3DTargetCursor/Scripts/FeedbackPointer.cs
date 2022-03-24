using UnityEngine;

namespace Graphics.Feedback.Scripts
{
    /// <summary>
    ///   Handles the feedback pointer position.
    /// </summary>
    public class FeedbackPointer
    {
        /// <summary>
        /// Instance of the move indicator created from _moveToIndicator.
        /// </summary>
        private static GameObject _moveToIndicatorInstance;

        /// <summary>
        ///   Creates an instance of given prefab and scales it.
        /// </summary>
        /// <param name="prefabInstance">Prefab to instantiate.</param>
        /// <param name="scale">Scale of the pointer.</param>
        public void PreparePointer(GameObject prefabInstance, float scale)
        {
            if (prefabInstance == null) return;

            if (_moveToIndicatorInstance == null)
                _moveToIndicatorInstance = Object.Instantiate(prefabInstance);

            _moveToIndicatorInstance.SetActive(false);
            _moveToIndicatorInstance.transform.localScale = Vector3.one * scale;

            Object.DontDestroyOnLoad(_moveToIndicatorInstance);
        }

        /// <summary>
        ///   Sets the world position of the pointer.
        /// </summary>
        /// <param name="position">World position.</param>
        public void ShowPointer(Vector3 position)
        {
            _moveToIndicatorInstance?.SetActive(false);
            if (_moveToIndicatorInstance != null) _moveToIndicatorInstance.transform.position = position;
            _moveToIndicatorInstance?.SetActive(true);
        }
    }
}