/*
 * @author Valentin Simonov / http://va.lent.in/
 */

using System;
using TouchScript.Pointers;
using UnityEngine;

namespace TouchScript.InputSources
{
    /// <summary>
    /// Base class for all pointer input sources.
    /// </summary>
    public abstract class InputSource : MonoBehaviour, IInputSource, IRemapableInputSource
    {
        #region Public properties

        /// <summary>
        /// Gets or sets current remapper.
        /// </summary>
        /// <value>Optional remapper to use to change screen coordinates which go into the TouchManager.</value>
        public ICoordinatesRemapper CoordinatesRemapper
        {
            get { return coordinatesRemapper; }
            set { coordinatesRemapper = value; }
        }

        #endregion

        #region Private variables

        [SerializeField]
        [HideInInspector]
        private bool advancedProps; // is used to save whether advanced properties are opened or closed

        private ICoordinatesRemapper coordinatesRemapper;
        private TouchManagerInstance manager;

        #endregion

        #region Public methods

        /// <inheritdoc />
        public virtual void UpdateInput() {}

        /// <inheritdoc />
        public virtual bool CancelPointer(Pointer pointer, bool @return)
        {
            return false;
        }

        #endregion

        #region Internal methods

        public virtual void INTERNAL_ReleasePointer(Pointer pointer) {}

        #endregion

        #region Unity methods

        /// <summary>
        /// Unity OnEnable callback.
        /// </summary>
        protected virtual void OnEnable()
        {
            manager = TouchManagerInstance.Instance;
            if (manager == null) throw new InvalidOperationException("TouchManager instance is required!");
            manager.AddInput(this);
        }

        /// <summary>
        /// Unity OnDestroy callback.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (manager != null)
            {
                manager.RemoveInput(this);
                manager = null;
            }
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Begin pointer in given screen position.
        /// </summary>
        /// <param name="position">Screen position.</param>
        /// <returns> New pointer. </returns>
        protected virtual void beginPointer(Pointer pointer, Vector2 position, bool remap = true)
        {
            if (coordinatesRemapper != null && remap) position = coordinatesRemapper.Remap(position);
            manager.INTERNAL_BeginPointer(pointer, position);
        }

        /// <summary>
        /// Mark pointer as updated.
        /// </summary>
        /// <param name="id">Pointer id.</param>
        protected virtual void updatePointer(int id)
        {
            manager.INTERNAL_UpdatePointer(id);
        }

        /// <summary>
        /// Mark pointer as moved.
        /// </summary>
        /// <param name="id">Pointer id.</param>
        /// <param name="position">Screen position.</param>
        protected virtual void movePointer(int id, Vector2 position)
        {
            if (coordinatesRemapper != null) position = coordinatesRemapper.Remap(position);
            manager.INTERNAL_MovePointer(id, position);
        }

        /// <summary>
        /// End pointer with id.
        /// </summary>
        /// <param name="id">Pointer id.</param>
        protected virtual void endPointer(int id)
        {
            manager.INTERNAL_EndPointer(id);
        }

        /// <summary>
        /// Cancel pointer with id.
        /// </summary>
        /// <param name="id">Pointer id.</param>
        protected virtual void cancelPointer(int id)
        {
            manager.INTERNAL_CancelPointer(id);
        }

        #endregion
    }
}