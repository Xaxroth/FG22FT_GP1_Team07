using System.Collections;
using UnityEngine;

public class BuffHandler : MonoBehaviour
{
    private PlayerMovement _playerMovement;
    private PlayerState _playerState;
    
    public void Init(PlayerMovement playerMovement, PlayerState playerState)
    {
        _playerMovement = playerMovement;
        _playerState = playerState;
    }

    /// <summary>
    /// Resets players velocity to 0
    /// </summary>
    public void ResetVelocity()
    {
        _playerMovement.ResetVelocity();
    }
    
    /// <summary>
    /// Pushes the player backwards towards the start of the level by the given distance
    /// </summary>
    /// <param name="force"></param>
    public void PushBackwards(float force)
    {
        _playerMovement.Push(-_playerMovement.transform.forward, force);
    }

    /// <summary>
    /// Pushes the player away from the given collisionPoint by the given force
    /// </summary>
    /// <param name="collisionPosition"></param>
    /// <param name="force"></param>
    public void PushBack(Vector3 collisionPosition, float force)
    {
        var position = _playerMovement.transform.position - collisionPosition;
        _playerMovement.Push(position, force);
    }

    /// <summary>
    /// Slows the player down by the given amount for the given time in seconds
    /// Defaults to 1 second
    /// Defaults to 0.5f multiplier
    /// </summary>
    /// <param name="time"></param>
    /// <param name="multiplier"></param>
    public void SlowDown(float time, float multiplier = 0.5f)
    {
        StartCoroutine(SlowDownCoroutine(time, multiplier));
    }
    
    IEnumerator SlowDownCoroutine(float time, float multiplier)
    {
        _playerMovement.MaxSpeed *= multiplier;
        yield return new WaitForSeconds(time);
        _playerMovement.MaxSpeed /= multiplier;
    }

    /// <summary>
    /// Pushes the player down by the given distance
    /// </summary>
    /// <param name="force"></param>
    /// <param name="resetVelocity"></param>
    public void PushDown(float force)
    {
        _playerMovement.Push(-_playerMovement.transform.up, force);
    }

    /// <summary>
    /// Slows down the player's horizontal movement by the given amount
    /// Defaults to 1 second
    /// Defaults to 0.5f multiplier
    /// </summary>
    /// <param name="time"></param>
    /// <param name="multiplier"></param>
    public void SlowDownHorizontalControls(float time, float multiplier = 0.5f)
    {
        StartCoroutine(SlowDownHorizontalControlsCoroutine(time, multiplier));
    }
    
    IEnumerator SlowDownHorizontalControlsCoroutine(float time, float multiplier)
    {
        _playerMovement.MoveHorizontalSpeed *= multiplier;
        yield return new WaitForSeconds(time);
        _playerMovement.MoveHorizontalSpeed /= multiplier;
    }
    
    /// <summary>
    /// Slows down the player's vertical movement by the given amount
    /// Defaults to 1 second
    /// Defaults to 0.5f multiplier
    /// </summary>
    /// <param name="time"></param>
    /// <param name="multiplier"></param>
    public void SlowDownVerticalControls(float time, float multiplier = 0.5f)
    {
        StartCoroutine(SlowDownVerticalControlsCoroutine(time, multiplier));
    }
    
    IEnumerator SlowDownVerticalControlsCoroutine(float time, float multiplier)
    {
        _playerMovement.MoveVerticalSpeed *= multiplier;
        yield return new WaitForSeconds(time);
        _playerMovement.MoveVerticalSpeed /= multiplier;
    }


    /// <summary>
    /// Reverses the player's controls for the given time in seconds
    /// </summary>
    /// <param name="reverse"></param>
    public void ReverseControls(float time)
    {
        StartCoroutine(ReverseControlsCoroutine(time));
    }

    IEnumerator ReverseControlsCoroutine(float time)
    {
        _playerMovement.ReverseControls = true;
        yield return new WaitForSeconds(time);
        _playerMovement.ReverseControls = false;
    }
    
    /// <summary>
    /// Increase the player's fuel by the given amount
    /// </summary>
    /// <param name="amount"></param>
    public void AddFuel(float amount)
    {
        _playerState.FuelSystem.RechargeFuel(amount);
    }

    /// <summary>
    /// Increases the player's maxspeed by the given amount for the given time in seconds
    /// If instantly is true the player's horizontal & vertical movement is also increased by the given amount 
    /// </summary>
    /// <param name="time"></param>
    /// <param name="multiplier"></param>
    /// <param name="instantly"></param>
    public void SpeedUp(float time, float multiplier, bool instantly)
    {
        StartCoroutine(SpeedUpCoroutine(time, multiplier, instantly));
    }
    
    IEnumerator SpeedUpCoroutine(float time, float multiplier, bool instantly)
    {
        _playerMovement.MaxSpeed *= multiplier;
        if (instantly)
        {
            _playerMovement.MoveHorizontalSpeed *= multiplier;
            _playerMovement.MoveVerticalSpeed *= multiplier;
        }
        yield return new WaitForSeconds(time);
        if (instantly)
        {
            _playerMovement.MoveHorizontalSpeed /= multiplier;
            _playerMovement.MoveVerticalSpeed /= multiplier;
        }
        _playerMovement.MaxSpeed /= multiplier;
    }

    /// <summary>
    /// Adds the given amount of shield charges to the player
    /// Defaults to 1 charge if no amount is given
    /// </summary>
    /// <param name="amount"></param>
    public void AddShieldCharge(int amount = 1)
    {
        _playerState.ShieldSystem.AddShieldCharge(amount);
    }

    /// <summary>
    /// Disables the player's shield for the given time in seconds
    /// </summary>
    /// <param name="time"></param>
    public void DisableControls(float time)
    {
        StartCoroutine(DisableControlsCoroutine(time));
    }
    
    IEnumerator DisableControlsCoroutine(float time)
    {
        _playerMovement.ControlsDisabled = true;
        yield return new WaitForSeconds(time);
        _playerMovement.ControlsDisabled = false;
    }

    /// <summary>
    /// Removes the given amount of shield charges from the player
    /// Defaults to 1 charge if no amount is given
    /// </summary>
    /// <param name="amount"></param>
    public void RemoveShieldCharge(int amount = 1)
    {
        _playerState.ShieldSystem.RemoveShieldCharge(amount);
    }

}
