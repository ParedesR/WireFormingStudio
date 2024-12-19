namespace Aim.WireFormingStudio.Modules.SmartEditor.ViewModels.DataPresenters
{
    #region Using Directives -------------------------------------------------------------------------------------------------------------------------

    using System;   
    using System.ComponentModel;

    using AosLibraries.SharedInterfaces.DomainData.WireEntities;

    using AosLibraries.Kernel.DomainData.Core.OrthodonticWires;
        
    #endregion Using Directives ----------------------------------------------------------------------------------------------------------------------
    
    public sealed class SegmentAsLrarDataProvider : ISegmentAsLrarDataProvider, INotifyPropertyChanged
    {
        #region Member Variables ---------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Number of the segment in the sequence
        /// </summary>
        private int _sequenceStep;

        /// <summary>
        /// Length of the segment
        /// </summary>
        private double _length;

        /// <summary>
        /// Speed to bend length
        /// </summary>
        private double _lengthSpeed;

        /// <summary>
        /// Rotation angle of the segment
        /// </summary>
        private double _rotation;
        
        /// <summary>
        /// Bend angle 
        /// </summary>
        private double _angle;

        /// <summary>
        /// Speed to bend angle
        /// </summary>
        private double _angleSpeed;

        /// <summary>
        /// Bending radius
        /// </summary>
        private double _radius;

        /// <summary>
        /// 
        /// </summary>
        private string _flags;

        /// <summary>
        /// 
        /// </summary>
        private string _inputOutput;

        /// <summary>
        /// 
        /// </summary>
        private string _notes;
        
        #endregion Member Variables ------------------------------------------------------------------------------------------------------------------

        #region Constructors -------------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sequenceStep"></param>
        /// <param name="segmentAsLrar"></param>
        public SegmentAsLrarDataProvider(int sequenceStep, IWireSegmentAsLrar segmentAsLrar)
        {
            SequenceStep = sequenceStep;

            SegmentAsLrar = new WireSegmentAsLrar(segmentAsLrar);

            Length = segmentAsLrar.Length;
            Rotation = segmentAsLrar.Rotation;
            Angle = segmentAsLrar.Angle;
            Radius = segmentAsLrar.Radius;                       
        }

        #endregion Constructors ---------------------------------------------------------------------------------------------------

        #region ISegmentAsLrarDataProvider Interface Implementation ---------------------------------------------------------------

        /// <summary>
        /// Sequence number in the set of segments
        /// </summary>
        public int SequenceStep
        {
            get => _sequenceStep;
            set
            {
                if (value == _sequenceStep)
                {
                    return;
                }

                _sequenceStep = value;

                NotifyPropertyChanged("SequenceStep");
            }
        }
        
        /// <summary>
        /// Length of the segment
        /// </summary>
        public double Length
        {
            get => _length;
            set
            {
                if (Math.Abs(value - _length) < double.Epsilon)
                {
                    return;
                }

                _length = value;
                SegmentAsLrar.Length = _length;

                NotifyPropertyChanged("Length");
            }
        }

        /// <summary>
        /// Speed to bend length
        /// </summary>
        public double LengthSpeed
        {
            get => _lengthSpeed;
            set
            {
                if (Math.Abs(value - _lengthSpeed) < double.Epsilon)
                {
                    return;
                }

                _lengthSpeed = value;
                
                NotifyPropertyChanged("LengthSpeed");
            }
        }

        /// <summary>
        /// Rotation angle of the segment
        /// </summary>
        public double Rotation
        {
            get => _rotation;
            set
            {
                if (Math.Abs(value - _rotation) < double.Epsilon)
                {
                    return;
                }

                _rotation = value;
                SegmentAsLrar.Rotation = _rotation;

                NotifyPropertyChanged("Rotation");
            }
        }

        /// <summary>
        /// Bend angle 
        /// </summary>
        public double Angle
        {
            get => _angle;
            set
            {
                if (Math.Abs(value - _angle) < double.Epsilon)
                {
                    return;
                }

                _angle = value;
                SegmentAsLrar.Angle = _angle;

                NotifyPropertyChanged("Angle");
            }
        }

        /// <summary>
        /// Speed to bend angle
        /// </summary>
        public double AngleSpeed
        {
            get => _angleSpeed;
            set
            {
                if (Math.Abs(value - _angleSpeed) < double.Epsilon)
                {
                    return;
                }

                _angleSpeed = value;

                NotifyPropertyChanged("AngleSpeed");
            }
        }

        /// <summary>
        /// Bending radius
        /// </summary>
        public double Radius
        {
            get => _radius;
            set
            {
                if (Math.Abs(value - _radius) < double.Epsilon)
                {
                    return;
                }

                _radius = value;
                SegmentAsLrar.Radius = _radius;

                NotifyPropertyChanged("Radius");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Flags
        {
            get => _flags;
            set
            {
                if (value == _flags)
                {
                    return;
                }

                _flags = value;

                NotifyPropertyChanged("Flags");
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        public string InputOutput
        {
            get => _inputOutput;
            set
            {
                if (value == _inputOutput)
                {
                    return;
                }

                _inputOutput = value;

                NotifyPropertyChanged("InputOutput");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Notes
        {
            get => _notes;
            set
            {
                if (value == _notes)
                {
                    return;
                }

                _notes = value;

                NotifyPropertyChanged("Notes");
            }
        }

        /// <summary>
        /// Wire segment being wrapped
        /// </summary>
        public IWireSegmentAsLrar SegmentAsLrar { get; }

        #region ICloneable Interface Implementation -------------------------------------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion ICloneable Interface Implementation ----------------------------------------------------------------------------

        #endregion ISegmentAsLrarDataProvider Interface Implementation ------------------------------------------------------------

        #region INotifyPropertyChanged Interface Implementation -------------------------------------------------------------------

        /// <summary>
        /// 
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        private void NotifyPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        #endregion INotifyPropertyChanged Interface Implementation ----------------------------------------------------------------
    }
}
