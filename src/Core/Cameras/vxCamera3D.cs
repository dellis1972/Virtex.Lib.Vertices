using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using vxVertices.Core.Input;
using vxVertices.Mathematics;
using vxVertices.Utilities;

namespace vxVertices.Core.Cameras
{
    /// <summary>
    /// Whether or not to use the default free-flight camera controls.
    /// Set to false when using vehicles or character controllers.
    /// </summary>
    public enum CameraType
    {
        Freeroam,
        CharacterFPS,
        ChaseCamera,
        Orbit
    }

    /// <summary>
    /// Simple Camera class.
    /// </summary>
	public class vxCamera3D
    {
        public CameraType CameraType = CameraType.Freeroam;
        
        private Matrix mViewMatrix;
        public Matrix View
        {
            get { return mViewMatrix; }
            set { mViewMatrix = value; }
        }

        private Matrix mProjectionMatrix;
        public Matrix Projection
        {
            get { return mProjectionMatrix; }
            set { mProjectionMatrix = value; }
        }

        private float mFieldOfView;
        public float FieldOfView
        {
            get { return mFieldOfView; }
            set { mFieldOfView = value; calcProjectionMatrix(); }
        }

        private float mAspectRatio;

        public float AspectRatio
        {
            get { return mAspectRatio; }
            set { mAspectRatio = value; calcProjectionMatrix(); }
        }

        private float mNearPlane;
        public float NearPlane
        {
            get { return mNearPlane; }
            set { mNearPlane = value; calcProjectionMatrix(); }
        }

        private float mFarPlane;
        public float FarPlane
        {
            get { return mFarPlane; }
            set { mFarPlane = value; calcProjectionMatrix(); }
        }
        
        /// <summary>
        /// Focal Distance Used During Depth of Field Calculations.
        /// </summary>
        public float FocalDistance
        {
            get { return _focalDistance; }
            set { _focalDistance = value;}
        }
        float _focalDistance = 40;


        /// <summary>
        /// Focal Width Used in the Depth of Field Calculations.
        /// </summary>
        public float FocalWidth
        {
            get { return _focalWidth; }
            set { _focalWidth = value; }
        }
        float _focalWidth = 75;

        /// <summary>
        /// Gets the world transformation of the camera.
        /// </summary>
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }
        private Matrix worldMatrix;

        /// <summary>
        /// Position of camera in world space.
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        private Vector3 position;

        /// <summary>
        /// Velocity of camera.
        /// </summary>
        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        private Vector3 velocity;


        /// <summary>
        /// Gets or sets the speed at which the camera moves for freem roaming.
        /// </summary>
        public float Speed { get { return speed; } set { speed = Math.Max(value, 0); } }
        private float speed;

        /// <summary>
        /// Gets or sets the yaw rotation of the camera.
        /// </summary>
        public float Yaw
        {
            get { return yaw; }
            set { yaw = MathHelper.WrapAngle(value); }
        }
        private float yaw;

        /// <summary>
        /// Gets or sets the pitch rotation of the camera.
        /// </summary>
        public float Pitch
        {
            get { return pitch; }
            set
            {
                pitch = value;
                if (pitch > MathHelper.PiOver2 * .99f)
                    pitch = MathHelper.PiOver2 * .99f;
                else if (pitch < -MathHelper.PiOver2 * .99f)
                    pitch = -MathHelper.PiOver2 * .99f;
            }
        }
        private float pitch;

        vxEngine vxEngine;

        #region Chase Camera Code

        #region Chased object properties (set externally each frame)

        /// <summary>
        /// Position of object being chased.
        /// </summary>
        public Vector3 ChasePosition
        {
            get { return chasePosition; }
            set { chasePosition = value; }
        }
        private Vector3 chasePosition;

        /// <summary>
        /// Direction the chased object is facing.
        /// </summary>
        public Vector3 ChaseDirection
        {
            get { return chaseDirection; }
            set { chaseDirection = value; }
        }
        private Vector3 chaseDirection;

        /// <summary>
        /// Chased object's Up vector.
        /// </summary>
        public Vector3 Up
        {
            get { return up; }
            set { up = value; }
        }
        private Vector3 up = Vector3.Up;

        #endregion

        #region Desired camera positioning (set when creating camera or changing view)

        /// <summary>
        /// Desired camera position in the chased object's coordinate system.
        /// </summary>
        public Vector3 DesiredPositionOffset
        {
            get { return desiredPositionOffset; }
            set { desiredPositionOffset = value; }
        }
        private Vector3 desiredPositionOffset = new Vector3(0, 2.0f, 2.0f);

        /// <summary>
        /// Desired camera position in world space.
        /// </summary>
        public Vector3 DesiredPosition
        {
            get
            {
                // Ensure correct value even if update has not been called this frame
                UpdateWorldPositions();

                return desiredPosition;
            }
        }
        private Vector3 desiredPosition;

        /// <summary>
        /// Look at point in the chased object's coordinate system.
        /// </summary>
        public Vector3 LookAtOffset
        {
            get { return lookAtOffset; }
            set { lookAtOffset = value; }
        }
        private Vector3 lookAtOffset = new Vector3(0, 2.8f, 0);

        /// <summary>
        /// Look at point in world space.
        /// </summary>
        public Vector3 LookAt
        {
            get
            {
                // Ensure correct value even if update has not been called this frame
                UpdateWorldPositions();

                return lookAt;
            }
        }
        private Vector3 lookAt;

        #endregion

        #region Camera physics (typically set when creating camera)

        /// <summary>
        /// Physics coefficient which controls the influence of the camera's position
        /// over the spring force. The stiffer the spring, the closer it will stay to
        /// the chased object.
        /// </summary>
        public float Stiffness
        {
            get { return stiffness; }
            set { stiffness = value; }
        }
        private float stiffness = 450000.0f;

        /// <summary>
        /// Physics coefficient which approximates internal friction of the spring.
        /// Sufficient damping will prevent the spring from oscillating infinitely.
        /// </summary>
        public float Damping
        {
            get { return damping; }
            set { damping = value; }
        }
        private float damping = 35000.0f;

        /// <summary>
        /// Mass of the camera body. Heaver objects require stiffer springs with less
        /// damping to move at the same rate as lighter objects.
        /// </summary>
        public float Mass
        {
            get { return mass; }
            set { mass = value; }
        }
        private float mass = 1000.0f;

        #endregion

        #endregion


		public vxCamera3D() { }

		public vxCamera3D(vxEngine vxEngine, Vector3 position, float pitch, float yaw, Matrix projectionMatrix, CameraType CameraType)
        {
            this.vxEngine = vxEngine;

            Position = position;
            Yaw = yaw;
            Pitch = pitch;
            Projection = projectionMatrix;

            Speed = 10;

            mFieldOfView = MathHelper.PiOver4;
            mAspectRatio = 600 / 800;
            mNearPlane = 0.1f;
            mFarPlane = 200;

            this.CameraType = CameraType;
            
            calcProjectionMatrix();

            if (CameraType == CameraType.ChaseCamera)
                Reset();
        }

        private void calcProjectionMatrix()
        {
            mProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(mFieldOfView, mAspectRatio, mNearPlane, mFarPlane);
        }


        public Vector3 OrbitTarget = Vector3.Zero;


        int mouseDelta = 0;
        int prevMsScrl = 0;
        

		public float OrbitZoom
		{
			get { return zoom; }
			set { zoom = value; }
		}
		float zoom = -15;

        public virtual void Update(GameTime time)
        {
            if (CameraType == CameraType.CharacterFPS)
            {
                //Only move around if the camera has control over its own position.
				float dt = ((float)time.ElapsedGameTime.Milliseconds)/1000;//0.0167f;
                //float distance = Speed * dt;

                    Yaw += (200- vxEngine.InputManager.MouseState.X) * dt * .12f;
                    Pitch += (200- vxEngine.InputManager.MouseState.Y) * dt * .12f;

				//Yaw += (vxEngine.InputManager.PreviousMouseState.X - vxEngine.InputManager.MouseState.X) * dt * .12f;
				//Pitch += (vxEngine.InputManager.PreviousMouseState.Y - vxEngine.InputManager.MouseState.Y) * dt * .12f;

                WorldMatrix = Matrix.CreateFromAxisAngle(Vector3.Right, Pitch) * Matrix.CreateFromAxisAngle(Vector3.Up, Yaw);
                WorldMatrix = WorldMatrix * Matrix.CreateTranslation(Position);
                View = Matrix.Invert(WorldMatrix);
                Mouse.SetPosition(200, 200);
                 
            }

            if (CameraType == CameraType.Orbit)
            {
                if (vxEngine.InputManager.IsNewMouseButtonPress(MouseButtons.MiddleButton))
                {
                    vxEngine.Mouse_ClickPos = new Vector2(vxEngine.InputManager.MouseState.X, vxEngine.InputManager.MouseState.Y);
                }

                if (vxEngine.InputManager.MouseState.MiddleButton == ButtonState.Pressed)
                {
                    float dt = 0.0167f;
                    Yaw += ((int)vxEngine.Mouse_ClickPos.X - vxEngine.InputManager.MouseState.X) * dt * .12f;
                    Pitch += ((int)vxEngine.Mouse_ClickPos.Y - vxEngine.InputManager.MouseState.Y) * dt * .12f;
                    Mouse.SetPosition((int)vxEngine.Mouse_ClickPos.X, (int)vxEngine.Mouse_ClickPos.Y);
                }

                    mouseDelta += vxEngine.InputManager.MouseState.ScrollWheelValue - prevMsScrl;
                    mouseDelta = Math.Max(mouseDelta, 15);
				zoom = vxSmooth.SmoothFloat(zoom, mouseDelta, 4);

                    WorldMatrix = Matrix.CreateTranslation(OrbitTarget + new Vector3(0, 0, (zoom) / 50));

                    WorldMatrix *= Matrix.CreateFromAxisAngle(Vector3.Right, Pitch) * Matrix.CreateFromAxisAngle(Vector3.Up, Yaw);

                    View = Matrix.Invert(WorldMatrix);
                    this.Position = WorldMatrix.Translation;

                    prevMsScrl = vxEngine.InputManager.MouseState.ScrollWheelValue;
            }

            if (CameraType == CameraType.Freeroam)
            {
                //Only move around if the camera has control over its own position.
                float dt = 0.0167f;
                float distance = Speed * dt/4;

                if (vxEngine.InputManager.MouseState.MiddleButton == ButtonState.Pressed)
                {
                    Yaw += ((int)vxEngine.Mouse_ClickPos.X - vxEngine.InputManager.MouseState.X) * dt * .12f;
                    Pitch += ((int)vxEngine.Mouse_ClickPos.Y - vxEngine.InputManager.MouseState.Y) * dt * .12f;
                }

                WorldMatrix = Matrix.CreateFromAxisAngle(Vector3.Right, Pitch) * Matrix.CreateFromAxisAngle(Vector3.Up, Yaw);
                
                if (vxEngine.InputManager.KeyboardState.IsKeyDown(Keys.W))
                    MoveForward(distance);
                if (vxEngine.InputManager.KeyboardState.IsKeyDown(Keys.S))
                    MoveForward(-distance);
                if (vxEngine.InputManager.KeyboardState.IsKeyDown(Keys.A))
                    MoveRight(-distance);
                if (vxEngine.InputManager.KeyboardState.IsKeyDown(Keys.D))
                    MoveRight(distance);
                if (vxEngine.InputManager.KeyboardState.IsKeyDown(Keys.Q))
                    Speed += 10;
                if (vxEngine.InputManager.KeyboardState.IsKeyDown(Keys.Z))
                    Speed -= 10;
                

                Speed = Math.Max(0, Speed);

                WorldMatrix = WorldMatrix * Matrix.CreateTranslation(Position);
                View = Matrix.Invert(WorldMatrix);
            }

            if (CameraType == CameraType.ChaseCamera)
            {
                UpdateWorldPositions();

                float elapsed = 0.0167f;// (float)gameTime.ElapsedGameTime.TotalSeconds;

                // Calculate spring force
                Vector3 stretch = Position - desiredPosition;
                Vector3 force = -stiffness * stretch - damping * Velocity;

                // Apply acceleration
                Vector3 acceleration = force / mass;

                Velocity += acceleration * elapsed;

                // Apply velocity
                Position += Velocity * elapsed;

                WorldMatrix = Matrix.CreateWorld(Position, View.Forward, View.Up);

                UpdateMatrices();
            }

            vxConsole.WriteToInGameDebug("Camera Position: " + this.Position);
        }


        /// <summary>
        /// Moves the camera forward.
        /// </summary>
        /// <param name="distance">Distance to move.</param>
        public void MoveForward(float distance)
        {
            Position += WorldMatrix.Forward * distance;
        }

        /// <summary>
        /// Moves the camera to the right.
        /// </summary>
        /// <param name="distance">Distance to move.</param>
        public void MoveRight(float distance)
        {
            Position += WorldMatrix.Right * distance;
        }

        /// <summary>
        /// Moves the camera up.
        /// </summary>
        /// <param name="distance">Distanec to move.</param>
        public void MoveUp(float distance)
        {
            Position += new Vector3(0, distance, 0);
        }


        /// <summary>
        /// Rebuilds object space values in world space. Invoke before publicly
        /// returning or privately accessing world space values.
        /// </summary>
        private void UpdateWorldPositions()
        {
            if (CameraType == CameraType.ChaseCamera)
            {
                // Construct a matrix to transform from object space to worldspace
                Matrix transform = Matrix.Identity;
                transform.Forward = ChaseDirection;
                transform.Up = Up;
                transform.Right = Vector3.Cross(Up, ChaseDirection);

                // Calculate desired camera properties in world space
                desiredPosition = ChasePosition +
                    Vector3.TransformNormal(DesiredPositionOffset, transform);
                lookAt = ChasePosition +
                    Vector3.TransformNormal(LookAtOffset, transform);
            }
        }

        /// <summary>
        /// Rebuilds camera's view and projection matricies.
        /// </summary>
        private void UpdateMatrices()
        {
            if (CameraType == CameraType.ChaseCamera)
            {
                View = Matrix.CreateLookAt(this.Position, this.LookAt, this.Up);
                Projection = Matrix.CreatePerspectiveFieldOfView(FieldOfView,
                    AspectRatio, NearPlane, FarPlane);
            }
        }

        public Matrix GetReflectionView(Plane SurfacePlane)
        {
            if (CameraType == CameraType.ChaseCamera)
            {
                Vector3 ReflcPos = new Vector3(
                       this.Position.X,
                       -this.Position.Y + SurfacePlane.D * 2,
                       this.Position.Z);

                Vector3 ReflcLookAt = new Vector3(
                       this.LookAt.X,
                       -this.LookAt.Y + SurfacePlane.D * 2,
                       this.LookAt.Z);

                Vector3 ReflcUp = Vector3.Reflect(WorldMatrix.Up, SurfacePlane.Normal);
                return Matrix.CreateLookAt(ReflcPos, ReflcLookAt, -ReflcUp);
            }
            else
            {
                Vector3 ReflcPos = new Vector3(
                    this.WorldMatrix.Translation.X,
                    -this.WorldMatrix.Translation.Y + SurfacePlane.D * 2,
                    this.WorldMatrix.Translation.Z);

                Vector3 ReflcLookAt = ReflcPos + new Vector3(
                    this.WorldMatrix.Forward.X,
                    -this.WorldMatrix.Forward.Y,
                    this.WorldMatrix.Forward.Z);

                Vector3 ReflcUp = Vector3.Reflect(WorldMatrix.Up, SurfacePlane.Normal);

                return Matrix.CreateLookAt(ReflcPos, ReflcLookAt, -ReflcUp);
            }
        }

        /// <summary>
        /// Forces camera to be at desired position and to stop moving. The is useful
        /// when the chased object is first created or after it has been teleported.
        /// Failing to call this after a large change to the chased object's position
        /// will result in the camera quickly flying across the world.
        /// </summary>
        public void Reset()
        {
            if (CameraType == CameraType.ChaseCamera)
            {
                UpdateWorldPositions();

                // Stop motion
                Velocity = Vector3.Zero;

                // Force desired position
                Position = desiredPosition;

                UpdateMatrices();
            }
        }
    }

}