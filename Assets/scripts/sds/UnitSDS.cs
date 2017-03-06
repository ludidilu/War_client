public class UnitSDS : CsvBase, IUnitSDS {

	public double moveSpeed;
	public double radius;
	public int weight;
	public double queuePos;

	public double GetMoveSpeed(){

		return moveSpeed;
	}

	public double GetRadius(){

		return radius;
	}

	public int GetWeight(){

		return weight;
	}

	public double GetQueuePos(){
		
		return queuePos;
	}
}
