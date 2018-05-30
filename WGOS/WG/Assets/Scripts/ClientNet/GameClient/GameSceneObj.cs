using System;
using Fm_ClientNet.Interface;

namespace Fm_ClientNet
{
    public class GameSceneObj : GameObj, IGameSceneObj
    {
        private float m_fPosiX = 0;
        private float m_fPosiY = 0;
        private float m_fPosiZ = 0;
        private float m_fOrient = 0;
        private float m_fDestX = 0;
        private float m_fDestY = 0;
        private float m_fDestZ = 0;
        private float m_fDestOrient = 0;
        private float m_fMoveSpeed = 0;
        private float m_fRotateSpeed = 0;
        private float m_fJumpSpeed = 0;
        private float m_fGravity = 0;
        int m_nMode = 1;
        string m_strLinkIdent;

        float m_fLinkX = 0;
        float m_fLinkY = 0;
        float m_fLinkZ = 0;
        float m_fLinkOrient = 0;

        public GameSceneObj()
        {
            m_fRotateSpeed = 100.0f;
            m_fJumpSpeed = 0.0f;
            m_fGravity = 0.0f;
        }


        public float GetPosiX()
        {
            return this.m_fPosiX;
        }

        public float GetPosiY()
        {
            return this.m_fPosiY;
        }

        public float GetPosiZ()
        {
            return this.m_fPosiZ;
        }

        public float GetOrient()
        {
            return this.m_fOrient;
        }

        public float GetDestX()
        {
            return this.m_fDestX;
        }

        public float GetDestY()
        {
            return this.m_fDestY;
        }

        public float GetDestZ()
        {
            return this.m_fDestZ;
        }

        public float GetDestOrient()
        {
            return this.m_fDestOrient;
        }

        public float GetMoveSpeed()
        {
            return this.m_fMoveSpeed;
        }

        public float GetRotateSpeed()
        {
            return this.m_fRotateSpeed;
        }

        public float GetJumpSpeed()
        {
            return this.m_fJumpSpeed;
        }

        public float GetGravity()
        {
            return this.m_fGravity;
        }

        public void SetMode(int value)
        {
            this.m_nMode = value;
        }

        public int GetMode()
        {
            return this.m_nMode;
        }

        public bool SetLocation(float x, float y, float z, float orient)
        {
            m_fPosiX = x;
            m_fPosiY = y;
            m_fPosiZ = z;
            m_fOrient = orient;
            return true;
        }

        public bool SetDestination(float x, float y, float z, float orient,
            float move_speed, float rotate_speed, float jump_speed, float gravity)
        {
            m_fDestX = x;
            //m_fDestY = y;
            m_fDestZ = z;
            m_fDestOrient = orient;
            m_fMoveSpeed = move_speed;
            m_fRotateSpeed = rotate_speed;
            m_fJumpSpeed = jump_speed;
            m_fGravity = gravity;
            return true;
        }

        public void SetLinkIdent(string value)
        {
            this.m_strLinkIdent = value;
        }



        public string GetLinkIdent()
        {
            return this.m_strLinkIdent;
        }

        public float GetLinkX()
        {
            return this.m_fLinkX;
        }

        public float GetLinkY()
        {
            return this.m_fLinkY;
        }

        public float GetLinkZ()
        {
            return this.m_fLinkZ;
        }

        public float GetLinkOrient()
        {
            return this.m_fLinkOrient;
        }

        public bool SetLinkPos(float x, float y, float z, float orient)
        {
            m_fLinkX = x;
            m_fLinkY = y;
            m_fLinkZ = z;
            m_fLinkOrient = orient;
            return true;
        }
    }
}



