cd ..\..\Runable
python guess.py --class_type gender --model_type inception --model_dir ..\Inception --filename %1 --face_detection_model ..\face_detection\haarcascade_frontalface_default.xml > ..\Result\result.txt
echo end >> ..\Result\result.txt
exit