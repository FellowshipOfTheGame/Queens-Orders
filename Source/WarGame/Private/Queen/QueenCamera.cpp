// Fill out your copyright notice in the Description page of Project Settings.

#include "WarGame.h"
#include "QueenCamera.h"

const FName AQueenCamera::MoveForwardBinding("CameraVertical");
const FName AQueenCamera::MoveRightBinding("CameraHorizontal");

AQueenCamera::AQueenCamera(const FObjectInitializer& ObjectInitializer)
	: Super(ObjectInitializer)
{
	// Criar camera
	camera = ObjectInitializer.CreateDefaultSubobject<UCameraComponent>(this, TEXT("CameraRTS"));
	//Camera->AttachTo(SpringArm, USpringArmComponent::SocketName);
	camera->bUsePawnControlRotation = false; // Don't rotate camera with controller

	FTransform transform;
	transform.SetLocation(FVector(-70000.0, 0, 7000));
	transform.SetRotation(FQuat(FRotator(-60.f, 0.f, 0.f)));
	camera->SetWorldTransform(transform);

}

void AQueenCamera::Tick(float DeltaSeconds)
{
	FVector Vector = camera->GetComponentLocation();
	float MoveSpeed = 30000.0f + Vector.X;

	// Find movement direction
	const float ForwardValue = GetInputAxisValue(MoveForwardBinding);
	const float RightValue = GetInputAxisValue(MoveRightBinding);

	// Clamp max size so that (X=1, Y=1) doesn't cause faster movement in diagonal directions
	const FVector MoveDirection = FVector(ForwardValue, RightValue, 0.f).ClampMaxSize(1.0f);

	// Calculate  movement
	const FVector Movement = MoveDirection * MoveSpeed * DeltaSeconds;

	//camera->SetWorldLocation(camera->GetComponentLocation() + Movement);

	// GEngine->AddOnScreenDebugMessage(-1, 15.0f, FColor::Red, FString::Printf(TEXT("Direction: %f %f"), Movement.X, Movement.Y )	);

}